import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';

@Component({
  selector: 'app-shell',
  imports: [
    MatButtonModule,
    MatIconModule,
    MatToolbarModule,
    RouterLink,
    RouterLinkActive,
    RouterOutlet,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="shell">
      <mat-toolbar class="shell__toolbar">
        <a class="brand" routerLink="/" aria-label="Pizza Place home">
          <mat-icon aria-hidden="true">local_pizza</mat-icon>
          <span>Pizza Place</span>
        </a>

        <nav class="shell__nav" aria-label="Primary">
          <a mat-button routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">
            Sales dashboard
          </a>
        </nav>
      </mat-toolbar>

      <main class="shell__content" id="main-content">
        <router-outlet />
      </main>
    </div>
  `,
  styles: `
    .shell {
      min-height: 100vh;
      background:
        radial-gradient(circle at top left, rgba(234, 88, 12, 0.16), transparent 34%),
        radial-gradient(circle at top right, rgba(15, 118, 110, 0.12), transparent 28%),
        var(--mat-sys-surface);
    }

    .shell__toolbar {
      position: sticky;
      top: 0;
      z-index: 10;
      background: color-mix(in srgb, var(--mat-sys-surface) 88%, transparent);
      backdrop-filter: blur(12px);
      border-bottom: 1px solid color-mix(in srgb, var(--mat-sys-outline-variant) 70%, transparent);
      gap: 1rem;
    }

    .brand {
      display: inline-flex;
      align-items: center;
      gap: 0.45rem;
      color: inherit;
      text-decoration: none;
      font: var(--mat-sys-title-large);
      margin-right: 1rem;
    }

    .brand mat-icon {
      color: var(--brand-primary);
    }

    .shell__nav a.active {
      background: color-mix(in srgb, var(--brand-primary) 12%, transparent);
    }

    .shell__content {
      width: min(1200px, calc(100% - 2rem));
      margin: 0 auto;
      padding: 1.5rem 0 3rem;
    }
  `,
})
export class ShellComponent {}
