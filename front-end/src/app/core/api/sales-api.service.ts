import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  DashboardQueryParams,
  PagedResult,
  SalesDashboard,
  SalesFilterOptions,
  SalesLineItem,
  SalesSearchParams,
} from './sales-api.models';

@Injectable({ providedIn: 'root' })
export class SalesApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl;

  getDashboard(params: DashboardQueryParams = {}): Observable<SalesDashboard> {
    return this.http.get<SalesDashboard>(`${this.baseUrl}/sales/dashboard`, {
      params: this.toHttpParams(params),
    });
  }

  searchSales(params: SalesSearchParams): Observable<PagedResult<SalesLineItem>> {
    return this.http.get<PagedResult<SalesLineItem>>(`${this.baseUrl}/sales`, {
      params: this.toHttpParams(params),
    });
  }

  getFilterOptions(): Observable<SalesFilterOptions> {
    return this.http.get<SalesFilterOptions>(`${this.baseUrl}/sales/filters`);
  }

  private toHttpParams(
    params: DashboardQueryParams | SalesSearchParams,
  ): HttpParams {
    let httpParams = new HttpParams();

    for (const [key, value] of Object.entries(params)) {
      if (value === null || value === undefined || value === '') {
        continue;
      }

      httpParams = httpParams.set(key, String(value));
    }

    return httpParams;
  }
}
