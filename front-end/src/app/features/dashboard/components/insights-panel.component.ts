import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

import { SalesInsights } from '../../../core/api/sales-api.models';
import { formatHourLabel } from '../../../shared/utils/date.utils';

@Component({
  selector: 'app-insights-panel',
  imports: [MatCardModule, MatIconModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <mat-card class="insights" appearance="outlined">
      <h2>Business insights</h2>
      <p class="lede">
        Actionable signals derived from the selected date range to guide staffing, menu emphasis,
        and promotions.
      </p>

      <ul>
        @for (item of items(); track item.label) {
          <li>
            <mat-icon aria-hidden="true">{{ item.icon }}</mat-icon>
            <div>
              <strong>{{ item.label }}</strong>
              <p>{{ item.value }}</p>
            </div>
          </li>
        }
      </ul>
    </mat-card>
  `,
  styles: `
    .insights {
      padding: 1.25rem;
      border-radius: 16px;
      height: 100%;
    }

    h2 {
      margin: 0;
      font: var(--mat-sys-title-large);
    }

    .lede {
      margin: 0.35rem 0 1rem;
      color: var(--mat-sys-on-surface-variant);
    }

    ul {
      list-style: none;
      margin: 0;
      padding: 0;
      display: grid;
      gap: 0.85rem;
    }

    li {
      display: flex;
      gap: 0.75rem;
      align-items: flex-start;
      padding: 0.75rem;
      border-radius: 12px;
      background: color-mix(in srgb, var(--brand-primary) 8%, transparent);
    }

    mat-icon {
      color: var(--brand-primary);
    }

    strong {
      display: block;
    }

    p {
      margin: 0.15rem 0 0;
      color: var(--mat-sys-on-surface-variant);
    }
  `,
})
export class InsightsPanelComponent {
  readonly insights = input.required<SalesInsights>();

  readonly items = computed(() => {
    const insights = this.insights();
    return [
      {
        icon: 'event',
        label: 'Peak weekday',
        value: insights.peakWeekday
          ? `${insights.peakWeekday} drives the strongest revenue`
          : 'Not enough data in this range',
      },
      {
        icon: 'schedule',
        label: 'Peak hour',
        value: insights.peakHour !== null
          ? `${formatHourLabel(insights.peakHour)} is the busiest service window`
          : 'Not enough data in this range',
      },
      {
        icon: 'category',
        label: 'Top category',
        value: insights.topCategory
          ? `${insights.topCategory} leads category mix`
          : 'Not enough data in this range',
      },
      {
        icon: 'star',
        label: 'Best seller',
        value: insights.topPizzaName
          ? `${insights.topPizzaName} is the top revenue pizza`
          : 'Not enough data in this range',
      },
    ];
  });
}
