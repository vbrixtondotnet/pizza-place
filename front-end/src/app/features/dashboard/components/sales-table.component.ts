import { CurrencyPipe, DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  input,
  output,
} from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';

import { PagedResult, SalesLineItem } from '../../../core/api/sales-api.models';
import { EmptyStateComponent } from '../../../shared/components/empty-state.component';
import { LoadingStateComponent } from '../../../shared/components/loading-state.component';

@Component({
  selector: 'app-sales-table',
  imports: [
    CurrencyPipe,
    DatePipe,
    EmptyStateComponent,
    LoadingStateComponent,
    MatCardModule,
    MatPaginatorModule,
    MatSortModule,
    MatTableModule,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <mat-card class="table-card" appearance="outlined">
      <div class="table-card__header">
        <div>
          <h2>Sales detail</h2>
          <p>
            @if (sales(); as page) {
              Showing {{ page.items.length }} of {{ page.totalCount }} matching line items
            } @else {
              Search and browse pizza sales lines
            }
          </p>
        </div>
      </div>

      @if (loading()) {
        <app-loading-state message="Loading sales…" />
      } @else if (!sales()?.items?.length) {
        <app-empty-state
          title="No sales found"
          message="No line items match the current filters."
        />
      } @else {
        <div class="table-wrap">
          <table
            mat-table
            [dataSource]="sales()!.items"
            matSort
            [matSortActive]="sortBy()"
            [matSortDirection]="sortDirection()"
            (matSortChange)="onSort($event)"
            aria-label="Sales line items"
          >
            <ng-container matColumnDef="date">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Date</th>
              <td mat-cell *matCellDef="let row">{{ row.date | date: 'MMM d, y' }}</td>
            </ng-container>

            <ng-container matColumnDef="time">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Time</th>
              <td mat-cell *matCellDef="let row">{{ row.time }}</td>
            </ng-container>

            <ng-container matColumnDef="orderId">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Order</th>
              <td mat-cell *matCellDef="let row">#{{ row.orderId }}</td>
            </ng-container>

            <ng-container matColumnDef="pizzaName">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Pizza</th>
              <td mat-cell *matCellDef="let row">
                <div class="primary-cell">{{ row.pizzaName }}</div>
                <div class="secondary-cell">{{ row.pizzaId }}</div>
              </td>
            </ng-container>

            <ng-container matColumnDef="category">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Category</th>
              <td mat-cell *matCellDef="let row">{{ row.category }}</td>
            </ng-container>

            <ng-container matColumnDef="size">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Size</th>
              <td mat-cell *matCellDef="let row">{{ row.size }}</td>
            </ng-container>

            <ng-container matColumnDef="quantity">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Qty</th>
              <td mat-cell *matCellDef="let row">{{ row.quantity }}</td>
            </ng-container>

            <ng-container matColumnDef="unitPrice">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Unit</th>
              <td mat-cell *matCellDef="let row">
                {{ row.unitPrice | currency: 'USD':'symbol':'1.2-2' }}
              </td>
            </ng-container>

            <ng-container matColumnDef="lineRevenue">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Line revenue</th>
              <td mat-cell *matCellDef="let row">
                <strong>{{ row.lineRevenue | currency: 'USD':'symbol':'1.2-2' }}</strong>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
          </table>
        </div>

        <mat-paginator
          [length]="sales()!.totalCount"
          [pageIndex]="pageIndex()"
          [pageSize]="sales()!.pageSize"
          [pageSizeOptions]="[10, 25, 50, 100]"
          (page)="onPage($event)"
          showFirstLastButtons
        />
      }
    </mat-card>
  `,
  styles: `
    .table-card {
      padding: 1.25rem;
      border-radius: 16px;
    }

    .table-card__header h2 {
      margin: 0;
      font: var(--mat-sys-title-large);
    }

    .table-card__header p {
      margin: 0.25rem 0 1rem;
      color: var(--mat-sys-on-surface-variant);
    }

    .table-wrap {
      overflow: auto;
    }

    table {
      width: 100%;
      min-width: 920px;
    }

    .primary-cell {
      font-weight: 500;
    }

    .secondary-cell {
      color: var(--mat-sys-on-surface-variant);
      font: var(--mat-sys-body-small);
    }
  `,
})
export class SalesTableComponent {
  readonly sales = input<PagedResult<SalesLineItem> | null>(null);
  readonly loading = input(false);
  readonly sortBy = input('date');
  readonly sortDirection = input<'asc' | 'desc'>('desc');

  readonly pageChange = output<{ page: number; pageSize: number }>();
  readonly sortChange = output<{ sortBy: string; sortDirection: 'asc' | 'desc' }>();

  readonly displayedColumns = [
    'date',
    'time',
    'orderId',
    'pizzaName',
    'category',
    'size',
    'quantity',
    'unitPrice',
    'lineRevenue',
  ];

  readonly pageIndex = computed(() => Math.max((this.sales()?.page ?? 1) - 1, 0));

  onPage(event: PageEvent): void {
    this.pageChange.emit({
      page: event.pageIndex + 1,
      pageSize: event.pageSize,
    });
  }

  onSort(sort: Sort): void {
    if (!sort.active || !sort.direction) {
      return;
    }

    this.sortChange.emit({
      sortBy: sort.active,
      sortDirection: sort.direction,
    });
  }
}
