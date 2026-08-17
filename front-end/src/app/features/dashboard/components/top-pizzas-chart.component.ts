import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ChartConfiguration } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

import { TopPizza } from '../../../core/api/sales-api.models';

@Component({
  selector: 'app-top-pizzas-chart',
  imports: [BaseChartDirective, MatCardModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <mat-card class="chart-card" appearance="outlined">
      <h2>Top pizzas by revenue</h2>
      <p>{{ summary() }}</p>
      <div class="chart-wrap" role="img" [attr.aria-label]="summary()">
        <canvas
          baseChart
          [data]="chartData()"
          [options]="chartOptions"
          [type]="'bar'"
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
      min-height: 280px;
    }
  `,
})
export class TopPizzasChartComponent {
  private readonly currencyPipe = new CurrencyPipe('en-US');
  private readonly decimalPipe = new DecimalPipe('en-US');

  readonly pizzas = input.required<TopPizza[]>();

  readonly summary = computed(() => {
    const pizzas = this.pizzas();
    if (!pizzas.length) {
      return 'No pizza sales in the selected range.';
    }

    const top = pizzas[0];
    return `${top.pizzaName} leads with ${this.decimalPipe.transform(top.quantitySold)} sold and ${this.currencyPipe.transform(top.revenue, 'USD', 'symbol', '1.0-0')} revenue.`;
  });

  readonly chartData = computed<ChartConfiguration<'bar'>['data']>(() => {
    const pizzas = [...this.pizzas()].reverse();
    return {
      labels: pizzas.map((pizza) => pizza.pizzaName),
      datasets: [
        {
          data: pizzas.map((pizza) => pizza.revenue),
          label: 'Revenue',
          backgroundColor: '#ea580c',
          borderRadius: 8,
          barThickness: 22,
        },
      ],
    };
  });

  readonly chartOptions: ChartConfiguration<'bar'>['options'] = {
    indexAxis: 'y',
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: (context) =>
            this.currencyPipe.transform(context.parsed.x, 'USD', 'symbol', '1.0-0') ?? '',
        },
      },
    },
    scales: {
      x: {
        beginAtZero: true,
        ticks: {
          callback: (value) =>
            this.currencyPipe.transform(Number(value), 'USD', 'symbol', '1.0-0') ?? '',
        },
      },
      y: {
        grid: { display: false },
      },
    },
  };
}
