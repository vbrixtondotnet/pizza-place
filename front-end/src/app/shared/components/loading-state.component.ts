import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-loading-state',
  imports: [MatProgressSpinnerModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="loading-state" role="status" aria-live="polite">
      <mat-spinner diameter="40" />
      <p>{{ message() }}</p>
    </div>
  `,
  styles: `
    .loading-state {
      display: grid;
      place-items: center;
      gap: 0.75rem;
      padding: 2rem 1rem;
      color: var(--mat-sys-on-surface-variant);
    }

    p {
      margin: 0;
    }
  `,
})
export class LoadingStateComponent {
  readonly message = input('Loading…');
}
