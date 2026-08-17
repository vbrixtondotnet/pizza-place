import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-empty-state',
  imports: [MatIconModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="empty-state" role="status">
      <mat-icon aria-hidden="true">inbox</mat-icon>
      <h3>{{ title() }}</h3>
      <p>{{ message() }}</p>
    </div>
  `,
  styles: `
    .empty-state {
      display: grid;
      place-items: center;
      gap: 0.35rem;
      padding: 2rem 1rem;
      text-align: center;
      color: var(--mat-sys-on-surface-variant);
    }

    mat-icon {
      font-size: 2rem;
      width: 2rem;
      height: 2rem;
      margin-bottom: 0.25rem;
    }

    h3,
    p {
      margin: 0;
    }

    h3 {
      color: var(--mat-sys-on-surface);
      font: var(--mat-sys-title-medium);
    }
  `,
})
export class EmptyStateComponent {
  readonly title = input('No results');
  readonly message = input('Try adjusting your filters or date range.');
}
