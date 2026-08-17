import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { environment } from '../../../environments/environment';
import { SalesApiService } from './sales-api.service';

describe('SalesApiService', () => {
  let service: SalesApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(SalesApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('maps dashboard query params', () => {
    service.getDashboard({ fromDate: '2015-01-01', toDate: '2015-01-31' }).subscribe();

    const request = httpMock.expectOne(
      `${environment.apiBaseUrl}/sales/dashboard?fromDate=2015-01-01&toDate=2015-01-31`,
    );
    expect(request.request.method).toBe('GET');
    request.flush({
      fromDate: '2015-01-01',
      toDate: '2015-01-31',
      kpis: {
        totalRevenue: 0,
        orderCount: 0,
        pizzasSold: 0,
        averageOrderValue: 0,
        averagePizzasPerOrder: 0,
        revenueChangePercent: null,
      },
      insights: {
        peakWeekday: null,
        peakHour: null,
        topCategory: null,
        topPizzaName: null,
      },
      dailyTrend: [],
      categoryBreakdown: [],
      topPizzas: [],
    });
  });

  it('forwards category, size, and search filters to the dashboard endpoint', () => {
    service
      .getDashboard({
        fromDate: '2015-01-01',
        toDate: '2015-01-31',
        query: 'chicken',
        category: 'Chicken',
        size: 'L',
      })
      .subscribe();

    const request = httpMock.expectOne(
      `${environment.apiBaseUrl}/sales/dashboard?fromDate=2015-01-01&toDate=2015-01-31&query=chicken&category=Chicken&size=L`,
    );
    expect(request.request.method).toBe('GET');
    request.flush({
      fromDate: '2015-01-01',
      toDate: '2015-01-31',
      kpis: {
        totalRevenue: 0,
        orderCount: 0,
        pizzasSold: 0,
        averageOrderValue: 0,
        averagePizzasPerOrder: 0,
        revenueChangePercent: null,
      },
      insights: {
        peakWeekday: null,
        peakHour: null,
        topCategory: null,
        topPizzaName: null,
      },
      dailyTrend: [],
      categoryBreakdown: [],
      topPizzas: [],
    });
  });

  it('omits empty search filters from the request', () => {
    service
      .searchSales({
        query: '',
        category: null,
        page: 2,
        pageSize: 25,
        sortBy: 'date',
        sortDirection: 'desc',
      })
      .subscribe();

    const request = httpMock.expectOne(
      `${environment.apiBaseUrl}/sales?page=2&pageSize=25&sortBy=date&sortDirection=desc`,
    );
    expect(request.request.method).toBe('GET');
    request.flush({
      items: [],
      page: 2,
      pageSize: 25,
      totalCount: 0,
      totalPages: 0,
    });
  });
});
