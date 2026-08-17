export function formatHourLabel(hour: number | null | undefined): string {
  if (hour === null || hour === undefined || Number.isNaN(hour)) {
    return 'N/A';
  }

  const normalized = ((hour % 24) + 24) % 24;
  const suffix = normalized >= 12 ? 'PM' : 'AM';
  const displayHour = normalized % 12 === 0 ? 12 : normalized % 12;
  return `${displayHour}:00 ${suffix}`;
}

export function toIsoDate(value: Date | string | null | undefined): string | null {
  if (!value) {
    return null;
  }

  if (typeof value === 'string') {
    return value.slice(0, 10);
  }

  const year = value.getFullYear();
  const month = `${value.getMonth() + 1}`.padStart(2, '0');
  const day = `${value.getDate()}`.padStart(2, '0');
  return `${year}-${month}-${day}`;
}

export function parseIsoDate(value: string | null | undefined): Date | null {
  if (!value) {
    return null;
  }

  const [year, month, day] = value.split('-').map(Number);
  if (!year || !month || !day) {
    return null;
  }

  return new Date(year, month - 1, day);
}
