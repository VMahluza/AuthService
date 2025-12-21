import { AxiosError } from 'axios';

/**
 * Extract error message from axios error
 * Single Responsibility: Error message extraction
 */
export function getErrorMessage(error: unknown): string {
  if (error instanceof AxiosError) {
    return (
      error.response?.data?.message ||
      error.response?.data?.detail ||
      error.response?.data?.title ||
      error.message ||
      'An error occurred'
    );
  }
  return error instanceof Error ? error.message : 'Unknown error occurred';
}

/**
 * Create authorization header with bearer token
 * Single Responsibility: Auth header creation
 */
export function createAuthHeader(token: string) {
  return {
    Authorization: `Bearer ${token}`,
  };
}
