import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KpiCardsComponent } from './kpi-cards.component';

describe('KpiCardsComponent', () => {
  let fixture: ComponentFixture<KpiCardsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KpiCardsComponent],
      providers: [CurrencyPipe, DecimalPipe],
    }).compileComponents();

    fixture = TestBed.createComponent(KpiCardsComponent);
    fixture.componentRef.setInput('kpis', {
      totalRevenue: 12345,
      orderCount: 100,
      pizzasSold: 250,
      averageOrderValue: 123.45,
      averagePizzasPerOrder: 2.5,
      revenueChangePercent: 12.5,
    });
    await fixture.whenStable();
  });

  it('renders key business KPI labels', () => {
    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('Revenue');
    expect(text).toContain('Orders');
    expect(text).toContain('Pizzas sold');
    expect(text).toContain('Avg order value');
    expect(text).toContain('+12.5% vs prior period');
  });
});
