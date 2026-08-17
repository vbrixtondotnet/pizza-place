import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-error-banner',
  imports: [MatButtonModule, MatIconModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="error-banner" role="alert">
      <div class="error-banner__content">
        <mat-icon aria-hidden="true">error</mat-icon>
        <div>
          <strong>{{ title() }}</strong>
          <p>{{ message() }}</p>
        </div>
      </div>
      @if (retryable()) {
        <button mat-stroked-button type="button" (click)="retry.emit()">Retry</button>
      }
    </div>
  `,
  styles: `
    .error-banner {
      display: flex;
      flex-wrap: wrap;
      align-items: center;
      justify-content: space-between;
      gap: 1rem;
      padding: 1rem 1.25rem;
      border-radius: 12px;
      background: color-mix(in srgb, var(--mat-sys-error) 12%, transparent);
      border: 1px solid color-mix(in srgb, var(--mat-sys-error) 35%, transparent);
      color: var(--mat-sys-on-surface);
    }

    .error-banner__content {
      display: flex;
      gap: 0.75rem;
      align-items: flex-start;
    }

    mat-icon {
      color: var(--mat-sys-error);
    }

    p {
      margin: 0.15rem 0 0;
      color: var(--mat-sys-on-surface-variant);
    }
  `,
})
export class ErrorBannerComponent {
  readonly title = input('Unable to load data');
  readonly message = input.required<string>();
  readonly retryable = input(true);
  readonly retry = output<void>();
}
