import { AxiosError } from 'axios';

/**
 * Generic API response type
 */
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
  cancelled?: boolean;
}

/**
 * Cache entry structure
 */
export interface CacheEntry<T> {
  data: T;
  timestamp: number;
  expiresAt: number;
}

/**
 * Request cancellation structure
 */
export interface CancellableRequest {
  requestId: string;
  signal: AbortSignal;
}

/**
 * Failed queue item for token refresh
 */
export interface FailedQueueItem {
  resolve: (token: string) => void;
  reject: (error: AxiosError) => void;
}

/**
 * Cache statistics
 */
export interface CacheStats {
  total: number;
  valid: number;
  expired: number;
  size: number;
}
