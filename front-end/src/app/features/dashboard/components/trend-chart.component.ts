import { CurrencyPipe, DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  input,
} from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ChartConfiguration } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

import { DailySalesPoint } from '../../../core/api/sales-api.models';

@Component({
  selector: 'app-trend-chart',
  imports: [BaseChartDirective, MatCardModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <mat-card class="chart-card" appearance="outlined">
      <div class="chart-card__header">
        <div>
          <h2>Daily revenue trend</h2>
          <p>{{ summary() }}</p>
        </div>
      </div>
      <div class="chart-wrap" role="img" [attr.aria-label]="summary()">
        <canvas
          baseChart
          [data]="chartData()"
          [options]="chartOptions"
          [type]="'line'"
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

    .chart-card__header h2 {
      margin: 0;
      font: var(--mat-sys-title-large);
    }

    .chart-card__header p {
      margin: 0.25rem 0 1rem;
      color: var(--mat-sys-on-surface-variant);
    }

    .chart-wrap {
      position: relative;
      min-height: 280px;
    }
  `,
})
export class TrendChartComponent {
  private readonly currencyPipe = new CurrencyPipe('en-US');
  private readonly datePipe = new DatePipe('en-US');

  readonly points = input.required<DailySalesPoint[]>();

  readonly summary = computed(() => {
    const points = this.points();
    if (!points.length) {
      return 'No daily sales in the selected range.';
    }

    const peak = [...points].sort((a, b) => b.revenue - a.revenue)[0];
    return `Peak day ${this.datePipe.transform(peak.date, 'MMM d, y')} at ${this.currencyPipe.transform(peak.revenue, 'USD', 'symbol', '1.0-0')}.`;
  });

  readonly chartData = computed<ChartConfiguration<'line'>['data']>(() => {
    const points = this.points();
    return {
      labels: points.map((point) => this.datePipe.transform(point.date, 'MMM d') ?? point.date),
      datasets: [
        {
          data: points.map((point) => point.revenue),
          label: 'Revenue',
          fill: true,
          tension: 0.35,
          borderColor: '#c2410c',
          backgroundColor: 'rgba(194, 65, 12, 0.15)',
          pointBackgroundColor: '#c2410c',
          pointRadius: points.length > 60 ? 0 : 3,
        },
      ],
    };
  });

  readonly chartOptions: ChartConfiguration<'line'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: (context) =>
            this.currencyPipe.transform(context.parsed.y, 'USD', 'symbol', '1.0-0') ?? '',
        },
      },
    },
    scales: {
      x: {
        grid: { display: false },
        ticks: {
          maxTicksLimit: 8,
        },
      },
      y: {
        beginAtZero: true,
        ticks: {
          callback: (value) =>
            this.currencyPipe.transform(Number(value), 'USD', 'symbol', '1.0-0') ?? '',
        },
      },
    },
  };
}
