/**
 * API Client Module - Main Entry Point
 * Exports all public APIs following Single Responsibility and Separation of Concerns
 */

// Initialize interceptors
import { setupInterceptors } from './interceptors';
setupInterceptors();

// Export axios instance for advanced usage
export { apiClient as default } from './config';

// Export HTTP methods
export { get, post, put, del } from './http-methods';

// Export types
export type { ApiResponse, CacheStats } from './types';

// Export cache utilities
export { clearCache, getCacheStats } from './cache';

// Export cancellation utilities
export {
  createCancellableRequest,
  cancelRequest,
  cancelAllRequests,
  cancelRequestsByUrl,
  getActiveRequestCount,
} from './cancellation';

// Export utility functions
export { getErrorMessage, createAuthHeader } from './utils';
