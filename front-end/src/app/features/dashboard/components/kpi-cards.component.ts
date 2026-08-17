import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { CurrencyPipe, DecimalPipe } from '@angular/common';

import { SalesKpi } from '../../../core/api/sales-api.models';

interface KpiCard {
  label: string;
  value: string;
  hint: string;
  icon: string;
  tone: 'neutral' | 'up' | 'down';
}

@Component({
  selector: 'app-kpi-cards',
  imports: [MatCardModule, MatIconModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="kpi-grid" aria-label="Sales KPIs">
      @for (card of cards(); track card.label) {
        <mat-card class="kpi-card" appearance="outlined">
          <div class="kpi-card__header">
            <span>{{ card.label }}</span>
            <mat-icon [attr.aria-hidden]="true">{{ card.icon }}</mat-icon>
          </div>
          <strong class="kpi-card__value">{{ card.value }}</strong>
          <p class="kpi-card__hint" [class.up]="card.tone === 'up'" [class.down]="card.tone === 'down'">
            {{ card.hint }}
          </p>
        </mat-card>
      }
    </section>
  `,
  styles: `
    .kpi-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
      gap: 1rem;
    }

    .kpi-card {
      padding: 1rem 1.1rem;
      border-radius: 16px;
      background: color-mix(in srgb, var(--mat-sys-surface-container-lowest) 80%, white);
    }

    .kpi-card__header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      color: var(--mat-sys-on-surface-variant);
      font: var(--mat-sys-label-large);
      margin-bottom: 0.65rem;
    }

    .kpi-card__value {
      display: block;
      font: var(--mat-sys-headline-small);
      letter-spacing: -0.02em;
    }

    .kpi-card__hint {
      margin: 0.45rem 0 0;
      color: var(--mat-sys-on-surface-variant);
      font: var(--mat-sys-body-small);
    }

    .kpi-card__hint.up {
      color: #1b7f4c;
    }

    .kpi-card__hint.down {
      color: #b42318;
    }
  `,
})
export class KpiCardsComponent {
  private readonly currencyPipe = new CurrencyPipe('en-US');
  private readonly decimalPipe = new DecimalPipe('en-US');

  readonly kpis = input.required<SalesKpi>();

  readonly cards = computed<KpiCard[]>(() => {
    const kpis = this.kpis();
    const change = kpis.revenueChangePercent;
    let changeHint = 'No prior-period comparison';
    let tone: KpiCard['tone'] = 'neutral';

    if (change !== null && change !== undefined) {
      const direction = change >= 0 ? 'up' : 'down';
      tone = change === 0 ? 'neutral' : direction;
      changeHint = `${change >= 0 ? '+' : ''}${this.decimalPipe.transform(change, '1.0-2')}% vs prior period`;
    }

    return [
      {
        label: 'Revenue',
        value: this.currencyPipe.transform(kpis.totalRevenue, 'USD', 'symbol', '1.0-0') ?? '$0',
        hint: changeHint,
        icon: 'payments',
        tone,
      },
      {
        label: 'Orders',
        value: this.decimalPipe.transform(kpis.orderCount, '1.0-0') ?? '0',
        hint: 'Distinct orders in range',
        icon: 'receipt_long',
        tone: 'neutral',
      },
      {
        label: 'Pizzas sold',
        value: this.decimalPipe.transform(kpis.pizzasSold, '1.0-0') ?? '0',
        hint: 'Total pizza quantity',
        icon: 'local_pizza',
        tone: 'neutral',
      },
      {
        label: 'Avg order value',
        value: this.currencyPipe.transform(kpis.averageOrderValue, 'USD', 'symbol', '1.2-2') ?? '$0',
        hint: 'Revenue per order',
        icon: 'shopping_bag',
        tone: 'neutral',
      },
      {
        label: 'Pizzas / order',
        value: this.decimalPipe.transform(kpis.averagePizzasPerOrder, '1.0-2') ?? '0',
        hint: 'Basket size signal',
        icon: 'restaurant',
        tone: 'neutral',
      },
    ];
  });
}
