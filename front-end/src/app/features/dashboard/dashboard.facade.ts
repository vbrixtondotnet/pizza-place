import { Injectable, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import {
  catchError,
  debounceTime,
  distinctUntilChanged,
  finalize,
  firstValueFrom,
  of,
  switchMap,
  tap,
} from 'rxjs';

import { getErrorMessage } from '../../core/api/error-utils';
import {
  PagedResult,
  SalesDashboard,
  SalesFilterOptions,
  SalesLineItem,
  SalesSearchParams,
} from '../../core/api/sales-api.models';
import { SalesApiService } from '../../core/api/sales-api.service';

export interface DashboardFilters {
  fromDate: string | null;
  toDate: string | null;
  query: string;
  category: string | null;
  size: string | null;
  page: number;
  pageSize: number;
  sortBy: string;
  sortDirection: 'asc' | 'desc';
}

const defaultFilters: DashboardFilters = {
  fromDate: null,
  toDate: null,
  query: '',
  category: null,
  size: null,
  page: 1,
  pageSize: 25,
  sortBy: 'date',
  sortDirection: 'desc',
};

@Injectable()
export class DashboardFacade {
  private readonly salesApi = inject(SalesApiService);

  private readonly filtersState = signal<DashboardFilters>({ ...defaultFilters });
  private readonly filterOptionsState = signal<SalesFilterOptions | null>(null);
  private readonly dashboardState = signal<SalesDashboard | null>(null);
  private readonly salesState = signal<PagedResult<SalesLineItem> | null>(null);
  private readonly dashboardLoadingState = signal(false);
  private readonly salesLoadingState = signal(false);
  private readonly dashboardErrorState = signal<string | null>(null);
  private readonly salesErrorState = signal<string | null>(null);
  private readonly initializedState = signal(false);
  private readonly enableRequests = signal(false);

  readonly filters = this.filtersState.asReadonly();
  readonly filterOptions = this.filterOptionsState.asReadonly();
  readonly dashboard = this.dashboardState.asReadonly();
  readonly sales = this.salesState.asReadonly();
  readonly dashboardLoading = this.dashboardLoadingState.asReadonly();
  readonly salesLoading = this.salesLoadingState.asReadonly();
  readonly dashboardError = this.dashboardErrorState.asReadonly();
  readonly salesError = this.salesErrorState.asReadonly();
  readonly initialized = this.initializedState.asReadonly();

  readonly hasActiveSearch = computed(() => {
    const filters = this.filtersState();
    return Boolean(filters.query || filters.category || filters.size);
  });

  private readonly dashboardRequest = toSignal(
    toObservable(
      computed(() => ({
        enabled: this.enableRequests(),
        fromDate: this.filtersState().fromDate,
        toDate: this.filtersState().toDate,
      })),
    ).pipe(
      distinctUntilChanged(
        (a, b) =>
          a.enabled === b.enabled && a.fromDate === b.fromDate && a.toDate === b.toDate,
      ),
      switchMap(({ enabled, fromDate, toDate }) => {
        if (!enabled) {
          return of(null);
        }

        this.dashboardLoadingState.set(true);
        this.dashboardErrorState.set(null);

        return this.salesApi.getDashboard({ fromDate, toDate }).pipe(
          tap((dashboard) => this.dashboardState.set(dashboard)),
          catchError((error) => {
            this.dashboardState.set(null);
            this.dashboardErrorState.set(getErrorMessage(error));
            return of(null);
          }),
          finalize(() => this.dashboardLoadingState.set(false)),
        );
      }),
    ),
    { initialValue: null },
  );

  private readonly salesRequest = toSignal(
    toObservable(
      computed(() => ({
        enabled: this.enableRequests(),
        filters: this.filtersState(),
      })),
    ).pipe(
      debounceTime(250),
      distinctUntilChanged((a, b) => JSON.stringify(a) === JSON.stringify(b)),
      switchMap(({ enabled, filters }) => {
        if (!enabled) {
          return of(null);
        }

        this.salesLoadingState.set(true);
        this.salesErrorState.set(null);

        return this.salesApi.searchSales(this.toSearchParams(filters)).pipe(
          tap((sales) => this.salesState.set(sales)),
          catchError((error) => {
            this.salesState.set(null);
            this.salesErrorState.set(getErrorMessage(error));
            return of(null);
          }),
          finalize(() => this.salesLoadingState.set(false)),
        );
      }),
    ),
    { initialValue: null },
  );

  async initializeFromQuery(params: Partial<DashboardFilters>): Promise<void> {
    try {
      const options = await firstValueFrom(this.salesApi.getFilterOptions());
      this.filterOptionsState.set(options);

      this.filtersState.set({
        ...defaultFilters,
        fromDate: params.fromDate ?? options.minDate,
        toDate: params.toDate ?? options.maxDate,
        query: params.query ?? '',
        category: params.category ?? null,
        size: params.size ?? null,
        page: params.page ?? 1,
        pageSize: params.pageSize ?? 25,
        sortBy: params.sortBy ?? 'date',
        sortDirection: params.sortDirection ?? 'desc',
      });
    } catch (error) {
      this.dashboardErrorState.set(getErrorMessage(error));
      this.filtersState.set({
        ...defaultFilters,
        ...params,
        query: params.query ?? '',
      });
    } finally {
      this.initializedState.set(true);
      this.enableRequests.set(true);
      void this.dashboardRequest;
      void this.salesRequest;
    }
  }

  patchFilters(patch: Partial<DashboardFilters>, options?: { resetPage?: boolean }): void {
    this.filtersState.update((current) => {
      const next = {
        ...current,
        ...patch,
      };

      if (options?.resetPage !== false && patch.page === undefined) {
        const pageResetKeys: (keyof DashboardFilters)[] = [
          'fromDate',
          'toDate',
          'query',
          'category',
          'size',
          'pageSize',
          'sortBy',
          'sortDirection',
        ];

        if (pageResetKeys.some((key) => key in patch)) {
          next.page = 1;
        }
      }

      return next;
    });
  }

  setPage(page: number): void {
    this.patchFilters({ page }, { resetPage: false });
  }

  setSort(sortBy: string, sortDirection: 'asc' | 'desc'): void {
    this.patchFilters({ sortBy, sortDirection });
  }

  clearSearchFilters(): void {
    this.patchFilters({
      query: '',
      category: null,
      size: null,
    });
  }

  reload(): void {
    this.filtersState.update((current) => ({ ...current }));
  }

  private toSearchParams(filters: DashboardFilters): SalesSearchParams {
    return {
      fromDate: filters.fromDate,
      toDate: filters.toDate,
      query: filters.query.trim() || null,
      category: filters.category,
      size: filters.size,
      page: filters.page,
      pageSize: filters.pageSize,
      sortBy: filters.sortBy,
      sortDirection: filters.sortDirection,
    };
  }
}
