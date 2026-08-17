import { ChangeDetectionStrategy, Component, effect, inject, input, output } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { debounceTime, distinctUntilChanged } from 'rxjs';

import { SalesFilterOptions } from '../../../core/api/sales-api.models';
import { parseIsoDate, toIsoDate } from '../../../shared/utils/date.utils';
import { DashboardFilters } from '../dashboard.facade';

@Component({
  selector: 'app-sales-filters',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
  ],
  providers: [provideNativeDateAdapter()],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <form class="filters" [formGroup]="form" (ngSubmit)="$event.preventDefault()">
      <mat-form-field appearance="outline">
        <mat-label>From</mat-label>
        <input matInput [matDatepicker]="fromPicker" formControlName="fromDate" />
        <mat-datepicker-toggle matIconSuffix [for]="fromPicker" />
        <mat-datepicker #fromPicker />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>To</mat-label>
        <input matInput [matDatepicker]="toPicker" formControlName="toDate" />
        <mat-datepicker-toggle matIconSuffix [for]="toPicker" />
        <mat-datepicker #toPicker />
      </mat-form-field>

      <mat-form-field appearance="outline" class="filters__search">
        <mat-label>Search sales</mat-label>
        <input
          matInput
          formControlName="query"
          placeholder="Pizza, category, order ID…"
        />
        <mat-icon matPrefix>search</mat-icon>
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Category</mat-label>
        <mat-select formControlName="category">
          <mat-option [value]="null">All categories</mat-option>
          @for (category of filterOptions()?.categories ?? []; track category) {
            <mat-option [value]="category">{{ category }}</mat-option>
          }
        </mat-select>
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Size</mat-label>
        <mat-select formControlName="size">
          <mat-option [value]="null">All sizes</mat-option>
          @for (size of filterOptions()?.sizes ?? []; track size) {
            <mat-option [value]="size">{{ size }}</mat-option>
          }
        </mat-select>
      </mat-form-field>

      <div class="filters__actions">
        <button mat-stroked-button type="button" (click)="clear.emit()">
          Clear search
        </button>
      </div>
    </form>
  `,
  styles: `
    .filters {
      display: grid;
      grid-template-columns: repeat(12, minmax(0, 1fr));
      gap: 0.75rem 1rem;
      align-items: start;
    }

    mat-form-field {
      grid-column: span 2;
      width: 100%;
    }

    .filters__search {
      grid-column: span 4;
    }

    .filters__actions {
      grid-column: span 2;
      display: flex;
      align-items: center;
      min-height: 56px;
    }

    @media (max-width: 1100px) {
      mat-form-field,
      .filters__search,
      .filters__actions {
        grid-column: span 6;
      }
    }

    @media (max-width: 720px) {
      mat-form-field,
      .filters__search,
      .filters__actions {
        grid-column: span 12;
      }
    }
  `,
})
export class SalesFiltersComponent {
  private readonly fb = inject(FormBuilder);

  readonly filters = input.required<DashboardFilters>();
  readonly filterOptions = input<SalesFilterOptions | null>(null);

  readonly filtersChange = output<Partial<DashboardFilters>>();
  readonly clear = output<void>();

  readonly form = this.fb.nonNullable.group({
    fromDate: this.fb.control<Date | null>(null),
    toDate: this.fb.control<Date | null>(null),
    query: '',
    category: this.fb.control<string | null>(null),
    size: this.fb.control<string | null>(null),
  });

  constructor() {
    effect(() => {
      const filters = this.filters();
      this.form.patchValue(
        {
          fromDate: parseIsoDate(filters.fromDate),
          toDate: parseIsoDate(filters.toDate),
          query: filters.query,
          category: filters.category,
          size: filters.size,
        },
        { emitEvent: false },
      );
    });

    this.form.valueChanges
      .pipe(
        debounceTime(200),
        distinctUntilChanged((a, b) => JSON.stringify(a) === JSON.stringify(b)),
        takeUntilDestroyed(),
      )
      .subscribe((value) => {
        this.filtersChange.emit({
          fromDate: toIsoDate(value.fromDate ?? null),
          toDate: toIsoDate(value.toDate ?? null),
          query: value.query ?? '',
          category: value.category ?? null,
          size: value.size ?? null,
        });
      });
  }
}
