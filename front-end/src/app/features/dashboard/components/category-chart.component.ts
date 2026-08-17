import { CurrencyPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ChartConfiguration } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

import { CategorySales } from '../../../core/api/sales-api.models';

@Component({
  selector: 'app-category-chart',
  imports: [BaseChartDirective, MatCardModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <mat-card class="chart-card" appearance="outlined">
      <h2>Category mix</h2>
      <p>{{ summary() }}</p>
      <div class="chart-wrap" role="img" [attr.aria-label]="summary()">
        <canvas
          baseChart
          [data]="chartData()"
          [options]="chartOptions"
          [type]="'doughnut'"
        ></canvas>
      </div>
    </mat-card>
  `,
  styles: `
    .chart-card {
      padding: 1.25rem;
      border-radius: 16px;
      height: 100%;
    }

    h2 {
      margin: 0;
      font: var(--mat-sys-title-large);
    }

    p {
      margin: 0.25rem 0 1rem;
      color: var(--mat-sys-on-surface-variant);
    }

    .chart-wrap {
      position: relative;
      min-height: 260px;
    }
  `,
})
export class CategoryChartComponent {
  private readonly currencyPipe = new CurrencyPipe('en-US');
  private readonly palette = ['#c2410c', '#ea580c', '#f59e0b', '#0f766e', '#7c2d12'];

  readonly categories = input.required<CategorySales[]>();

  readonly summary = computed(() => {
    const categories = this.categories();
    if (!categories.length) {
      return 'No category sales in the selected range.';
    }

    const top = categories[0];
    return `${top.category} accounts for ${top.revenueSharePercent}% of revenue (${this.currencyPipe.transform(top.revenue, 'USD', 'symbol', '1.0-0')}).`;
  });

  readonly chartData = computed<ChartConfiguration<'doughnut'>['data']>(() => {
    const categories = this.categories();
    return {
      labels: categories.map((item) => item.category),
      datasets: [
        {
          data: categories.map((item) => item.revenue),
          backgroundColor: categories.map((_, index) => this.palette[index % this.palette.length]),
          borderWidth: 0,
        },
      ],
    };
  });

  readonly chartOptions: ChartConfiguration<'doughnut'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'bottom',
      },
      tooltip: {
        callbacks: {
          label: (context) => {
            const label = context.label ?? '';
            const value =
              this.currencyPipe.transform(context.parsed, 'USD', 'symbol', '1.0-0') ?? '';
            return `${label}: ${value}`;
          },
        },
      },
    },
  };
}
