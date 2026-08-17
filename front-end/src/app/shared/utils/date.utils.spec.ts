import { describe, expect, it } from 'vitest';

import { formatHourLabel, parseIsoDate, toIsoDate } from './date.utils';

describe('date utils', () => {
  it('formats hours for business insights', () => {
    expect(formatHourLabel(0)).toBe('12:00 AM');
    expect(formatHourLabel(12)).toBe('12:00 PM');
    expect(formatHourLabel(18)).toBe('6:00 PM');
    expect(formatHourLabel(null)).toBe('N/A');
  });

  it('round-trips ISO dates without timezone drift', () => {
    const iso = '2015-07-04';
    const parsed = parseIsoDate(iso);

    expect(parsed).not.toBeNull();
    expect(toIsoDate(parsed)).toBe(iso);
  });
});
