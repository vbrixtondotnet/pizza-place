import { DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  inject,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';

import { ErrorBannerComponent } from '../../shared/components/error-banner.component';
import { LoadingStateComponent } from '../../shared/components/loading-state.component';
import { CategoryChartComponent } from './components/category-chart.component';
import { InsightsPanelComponent } from './components/insights-panel.component';
import { KpiCardsComponent } from './components/kpi-cards.component';
import { SalesFiltersComponent } from './components/sales-filters.component';
import { SalesTableComponent } from './components/sales-table.component';
import { TopPizzasChartComponent } from './components/top-pizzas-chart.component';
import { TrendChartComponent } from './components/trend-chart.component';
import { DashboardFacade, DashboardFilters } from './dashboard.facade';

@Component({
  selector: 'app-dashboard-page',
  imports: [
    DatePipe,
    CategoryChartComponent,
    ErrorBannerComponent,
    InsightsPanelComponent,
    KpiCardsComponent,
    LoadingStateComponent,
    MatCardModule,
    SalesFiltersComponent,
    SalesTableComponent,
    TopPizzasChartComponent,
    TrendChartComponent,
  ],
  providers: [DashboardFacade],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './dashboard.page.html',
  styleUrl: './dashboard.page.scss',
})
export class DashboardPageComponent implements OnInit {
  readonly facade = inject(DashboardFacade);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  ngOnInit(): void {
    const params = this.route.snapshot.queryParamMap;
    void this.facade.initializeFromQuery({
      fromDate: params.get('fromDate'),
      toDate: params.get('toDate'),
      query: params.get('query') ?? '',
      category: params.get('category'),
      size: params.get('size'),
      page: Number(params.get('page') ?? 1) || 1,
      pageSize: Number(params.get('pageSize') ?? 25) || 25,
      sortBy: params.get('sortBy') ?? 'date',
      sortDirection: params.get('sortDirection') === 'asc' ? 'asc' : 'desc',
    });
  }

  onFiltersChange(patch: Partial<DashboardFilters>): void {
    this.facade.patchFilters(patch);
    this.syncQueryParams();
  }

  onClearSearch(): void {
    this.facade.clearSearchFilters();
    this.syncQueryParams();
  }

  onPageChange(event: { page: number; pageSize: number }): void {
    this.facade.patchFilters(
      {
        page: event.page,
        pageSize: event.pageSize,
      },
      { resetPage: false },
    );
    this.syncQueryParams();
  }

  onSortChange(event: { sortBy: string; sortDirection: 'asc' | 'desc' }): void {
    this.facade.setSort(event.sortBy, event.sortDirection);
    this.syncQueryParams();
  }

  private syncQueryParams(): void {
    const filters = this.facade.filters();
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        fromDate: filters.fromDate,
        toDate: filters.toDate,
        query: filters.query || null,
        category: filters.category,
        size: filters.size,
        page: filters.page,
        pageSize: filters.pageSize,
        sortBy: filters.sortBy,
        sortDirection: filters.sortDirection,
      },
      queryParamsHandling: 'merge',
      replaceUrl: true,
    });
  }
}
