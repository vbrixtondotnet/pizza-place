import { HttpErrorResponse } from '@angular/common/http';

export function getErrorMessage(error: unknown, fallback = 'Something went wrong.'): string {
  if (error instanceof HttpErrorResponse) {
    if (error.status === 0) {
      return 'Unable to reach the Pizza Place API. Confirm the backend is running.';
    }

    const validationErrors = error.error?.errors as Record<string, string[]> | undefined;
    if (validationErrors) {
      return Object.values(validationErrors).flat().join(' ');
    }

    if (typeof error.error === 'string' && error.error.trim()) {
      return error.error;
    }

    if (typeof error.error?.title === 'string') {
      return error.error.title;
    }

    return error.message || fallback;
  }

  if (error instanceof Error && error.message) {
    return error.message;
  }

  return fallback;
}
