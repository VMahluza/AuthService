import axios, { AxiosError, AxiosInstance, InternalAxiosRequestConfig } from 'axios';
import { BACKEND_BASE_URL } from './constants';
import { getRefreshToken, setTokens, clearTokens } from './token-utils';

/**
 * Centralized API client using axios
 * Implements DRY and SOLID principles with automatic token refresh
 */

// Create axios instance with default configuration
const apiClient: AxiosInstance = axios.create({
  baseURL: BACKEND_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000, // 10 seconds
});

// Token refresh state management
let isRefreshing = false;
let failedQueue: Array<{
  resolve: (token: string) => void;
  reject: (error: AxiosError) => void;
}> = [];

// Request cancellation management
const activeRequests = new Map<string, AbortController>();
let requestIdCounter = 0;

const processQueue = (error: AxiosError | null, token: string | null = null) => {
  failedQueue.forEach((promise) => {
    if (error) {
      promise.reject(error);
    } else if (token) {
      promise.resolve(token);
    }
  });
  failedQueue = [];
};

/**
 * Refresh the access token using the refresh token from localStorage
 */
async function refreshAccessToken(): Promise<string | null> {
  try {
    if (process.env.NODE_ENV === 'development') {
      console.log('🔄 Token Refresh: Attempting to refresh access token...');
    }

    // Get refresh token (client-side only)
    const refreshToken = getRefreshToken();
    if (!refreshToken) {
      if (process.env.NODE_ENV === 'development') {
        console.warn('⚠️ Token Refresh: No refresh token found');
      }
      return null;
    }

    const response = await axios.post(`${BACKEND_BASE_URL}/auth/refresh`, {
      refreshToken,
    });

    const { accessToken, refreshToken: newRefreshToken } = response.data;

    // Store new tokens
    setTokens(accessToken, newRefreshToken || refreshToken);

    if (process.env.NODE_ENV === 'development') {
      console.log('✅ Token Refresh: Successfully refreshed access token');
    }

    return accessToken;
  } catch {
    if (process.env.NODE_ENV === 'development') {
      console.error('❌ Token Refresh: Failed to refresh token, clearing storage');
    }
    // Clear tokens if refresh fails
    clearTokens();
    return null;
  }
}

// Request interceptor to add auth token
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // Add request start time for performance tracking
    if (!config.headers) {
      config.headers = {} as InternalAxiosRequestConfig['headers'];
    }
    config.headers['request-startTime'] = Date.now().toString();

    // Development logging
    if (process.env.NODE_ENV === 'development') {
      console.log('🚀 API Request:', {
        method: config.method?.toUpperCase(),
        url: config.url,
        baseURL: config.baseURL,
        data: config.data,
        params: config.params,
        headers: {
          ...config.headers,
          Authorization: config.headers?.Authorization ? '***HIDDEN***' : undefined,
          'request-startTime': undefined, // Hide internal timing header
        },
      });
    }
    return config;
  },
  (error: AxiosError) => {
    if (process.env.NODE_ENV === 'development') {
      console.error('❌ Request Error:', error.message);
    }
    return Promise.reject(error);
  }
);

// Response interceptor for global error handling and automatic token refresh
apiClient.interceptors.response.use(
  (response) => {
    // Development logging
    if (process.env.NODE_ENV === 'development') {
      console.log('✅ API Response:', {
        method: response.config.method?.toUpperCase(),
        url: response.config.url,
        status: response.status,
        statusText: response.statusText,
        data: response.data,
        duration: response.config.headers?.['request-startTime']
          ? `${Date.now() - Number(response.config.headers['request-startTime'])}ms`
          : 'N/A',
      });
    }
    return response;
  },
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & { 
      _retry?: boolean;
      _retryCount?: number;
    };

    // Development error logging
    if (process.env.NODE_ENV === 'development') {
      console.error('❌ API Error:', {
        method: originalRequest?.method?.toUpperCase(),
        url: originalRequest?.url,
        status: error.response?.status,
        statusText: error.response?.statusText,
        message: error.message,
        data: error.response?.data,
      });
    }

    // Rate Limiting Handling (429 Too Many Requests)
    if (error.response?.status === 429) {
      const retryCount = originalRequest._retryCount || 0;
      const maxRetries = 3;

      if (retryCount < maxRetries) {
        // Get retry delay from Retry-After header or use exponential backoff
        const retryAfter = error.response.headers['retry-after'];
        let delay: number;

        if (retryAfter) {
          // Retry-After can be in seconds or a date
          const retryAfterNum = parseInt(retryAfter);
          if (!isNaN(retryAfterNum)) {
            delay = retryAfterNum * 1000; // Convert seconds to milliseconds
          } else {
            // Try parsing as date
            const retryDate = new Date(retryAfter);
            delay = retryDate.getTime() - Date.now();
          }
        } else {
          // Exponential backoff: 1s, 2s, 4s
          delay = Math.pow(2, retryCount) * 1000;
        }

        // Cap delay at 30 seconds
        delay = Math.min(delay, 30000);

        if (process.env.NODE_ENV === 'development') {
          console.log(`⏳ Rate Limit: Waiting ${delay}ms before retry (attempt ${retryCount + 1}/${maxRetries})`);
        }

        // Wait before retrying
        await new Promise((resolve) => setTimeout(resolve, delay));

        // Increment retry count
        originalRequest._retryCount = retryCount + 1;

        if (process.env.NODE_ENV === 'development') {
          console.log(`🔄 Rate Limit: Retrying request to ${originalRequest.url}`);
        }

        // Retry the request
        return apiClient(originalRequest);
      } else {
        if (process.env.NODE_ENV === 'development') {
          console.error('❌ Rate Limit: Max retries reached, giving up');
        }
        return Promise.reject(error);
      }
    }

    // If error is 401 and we haven't retried yet, try to refresh token
    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        // If already refreshing, queue this request
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then((token) => {
            if (originalRequest.headers) {
              originalRequest.headers['Authorization'] = `Bearer ${token}`;
            }
            return apiClient(originalRequest);
          })
          .catch((err) => {
            return Promise.reject(err);
          });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        const newAccessToken = await refreshAccessToken();

        if (newAccessToken) {
          // Update the authorization header with new token
          if (originalRequest.headers) {
            originalRequest.headers['Authorization'] = `Bearer ${newAccessToken}`;
          }

          processQueue(null, newAccessToken);

          // Retry the original request with new token
          return apiClient(originalRequest);
        } else {
          // Refresh failed, redirect to login
          processQueue(error, null);
          if (typeof window !== 'undefined') {
            window.location.href = '/auth/login';
          }
          return Promise.reject(error);
        }
      } catch (refreshError) {
        processQueue(error, null);
        if (typeof window !== 'undefined') {
          window.location.href = '/auth/login';
        }
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    // For other errors, just reject
    return Promise.reject(error);
  }
);

/**
 * Cache management for GET requests
 */
interface CacheEntry<T> {
  data: T;
  timestamp: number;
  expiresAt: number;
}

const cache = new Map<string, CacheEntry<unknown>>();
const DEFAULT_CACHE_TTL = 5 * 60 * 1000; // 5 minutes in milliseconds

/**
 * Generate cache key from URL and token
 */
function generateCacheKey(url: string, token?: string): string {
  return `${url}_${token ? 'authenticated' : 'public'}`;
}

/**
 * Check if cache entry is still valid
 */
function isCacheValid<T>(entry: CacheEntry<T>): boolean {
  return Date.now() < entry.expiresAt;
}

/**
 * Get data from cache
 */
function getFromCache<T>(key: string): T | null {
  const entry = cache.get(key) as CacheEntry<T> | undefined;
  
  if (!entry) {
    return null;
  }

  if (isCacheValid(entry)) {
    if (process.env.NODE_ENV === 'development') {
      console.log('💾 Cache Hit:', key);
    }
    return entry.data;
  }

  // Remove expired entry
  cache.delete(key);
  if (process.env.NODE_ENV === 'development') {
    console.log('🗑️ Cache Expired:', key);
  }
  return null;
}

/**
 * Store data in cache
 */
function setCache<T>(key: string, data: T, ttl: number = DEFAULT_CACHE_TTL): void {
  const entry: CacheEntry<T> = {
    data,
    timestamp: Date.now(),
    expiresAt: Date.now() + ttl,
  };
  cache.set(key, entry);
  
  if (process.env.NODE_ENV === 'development') {
    console.log('💾 Cache Set:', key, `(TTL: ${ttl}ms)`);
  }
}

/**
 * Clear cache for specific key or all cache
 */
export function clearCache(key?: string): void {
  if (key) {
    cache.delete(key);
    if (process.env.NODE_ENV === 'development') {
      console.log('🗑️ Cache Cleared:', key);
    }
  } else {
    cache.clear();
    if (process.env.NODE_ENV === 'development') {
      console.log('🗑️ Cache Cleared: All entries');
    }
  }
}

/**
 * Get cache statistics
 */
export function getCacheStats() {
  const entries = Array.from(cache.entries());
  const valid = entries.filter(([, entry]) => isCacheValid(entry));
  const expired = entries.length - valid.length;

  return {
    total: entries.length,
    valid: valid.length,
    expired,
    size: cache.size,
  };
}

/**
 * Generate unique request ID
 */
function generateRequestId(): string {
  return `req_${++requestIdCounter}_${Date.now()}`;
}

/**
 * Create an AbortController and register it
 * @param requestId - Optional custom request ID
 * @returns Object with requestId and signal
 */
export function createCancellableRequest(requestId?: string): { 
  requestId: string; 
  signal: AbortSignal;
} {
  const id = requestId || generateRequestId();
  const controller = new AbortController();
  activeRequests.set(id, controller);
  
  if (process.env.NODE_ENV === 'development') {
    console.log(`🎯 Request Registered: ${id} (${activeRequests.size} active)`);
  }
  
  return { requestId: id, signal: controller.signal };
}

/**
 * Cancel a specific request by ID
 * @param requestId - The request ID to cancel
 * @returns true if cancelled, false if not found
 */
export function cancelRequest(requestId: string): boolean {
  const controller = activeRequests.get(requestId);
  
  if (controller) {
    controller.abort();
    activeRequests.delete(requestId);
    
    if (process.env.NODE_ENV === 'development') {
      console.log(`🚫 Request Cancelled: ${requestId}`);
    }
    
    return true;
  }
  
  return false;
}

/**
 * Cancel all active requests
 * @returns Number of requests cancelled
 */
export function cancelAllRequests(): number {
  const count = activeRequests.size;
  
  activeRequests.forEach((controller, requestId) => {
    controller.abort();
    if (process.env.NODE_ENV === 'development') {
      console.log(`🚫 Request Cancelled: ${requestId}`);
    }
  });
  
  activeRequests.clear();
  
  if (process.env.NODE_ENV === 'development' && count > 0) {
    console.log(`🚫 All Requests Cancelled: ${count} total`);
  }
  
  return count;
}

/**
 * Cancel requests matching a URL pattern
 * @param pattern - URL pattern to match (string or regex)
 * @returns Number of requests cancelled
 */
export function cancelRequestsByUrl(pattern: string | RegExp): number {
  let cancelled = 0;
  
  activeRequests.forEach((controller, requestId) => {
    const matches = typeof pattern === 'string' 
      ? requestId.includes(pattern)
      : pattern.test(requestId);
    
    if (matches) {
      controller.abort();
      activeRequests.delete(requestId);
      cancelled++;
      
      if (process.env.NODE_ENV === 'development') {
        console.log(`🚫 Request Cancelled: ${requestId}`);
      }
    }
  });
  
  if (process.env.NODE_ENV === 'development' && cancelled > 0) {
    console.log(`🚫 Requests Cancelled by Pattern: ${cancelled} matched`);
  }
  
  return cancelled;
}

/**
 * Get active request count
 */
export function getActiveRequestCount(): number {
  return activeRequests.size;
}

/**
 * Cleanup completed request from tracking
 */
function cleanupRequest(requestId: string) {
  activeRequests.delete(requestId);
  
  if (process.env.NODE_ENV === 'development') {
    console.log(`✅ Request Completed: ${requestId} (${activeRequests.size} active)`);
  }
}

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
 * Extract error message from axios error
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
 */
export function createAuthHeader(token: string) {
  return {
    Authorization: `Bearer ${token}`,
  };
}

/**
 * Generic GET request handler with caching and cancellation
 * @param url - The API endpoint URL
 * @param token - Optional authentication token
 * @param useCache - Whether to use cache (default: true)
 * @param cacheTTL - Cache time-to-live in milliseconds (default: 5 minutes)
 * @param signal - Optional AbortSignal for cancellation
 * @param requestId - Optional custom request ID for tracking
 */
export async function get<T>(
  url: string,
  token?: string,
  useCache: boolean = true,
  cacheTTL?: number,
  signal?: AbortSignal,
  requestId?: string
): Promise<ApiResponse<T>> {
  let reqId = requestId;
  
  try {
    // Check cache first if enabled
    if (useCache) {
      const cacheKey = generateCacheKey(url, token);
      const cachedData = getFromCache<T>(cacheKey);
      
      if (cachedData !== null) {
        return { success: true, data: cachedData };
      }
    }

    // Setup cancellation if not provided
    const finalSignal = signal || (() => {
      const { requestId: id, signal: sig } = createCancellableRequest(reqId);
      reqId = id;
      return sig;
    })();

    // Make actual request
    const config = {
      ...(token ? { headers: createAuthHeader(token) } : {}),
      signal: finalSignal,
    };
    const response = await apiClient.get<T>(url, config);
    
    // Cleanup request tracking
    if (reqId) cleanupRequest(reqId);
    
    // Store in cache if enabled
    if (useCache) {
      const cacheKey = generateCacheKey(url, token);
      setCache(cacheKey, response.data, cacheTTL);
    }
    
    return { success: true, data: response.data };
  } catch (error) {
    // Cleanup request tracking
    if (reqId) cleanupRequest(reqId);
    
    // Check if request was cancelled
    if (error instanceof AxiosError && error.code === 'ERR_CANCELED') {
      if (process.env.NODE_ENV === 'development') {
        console.log(`🚫 Request Cancelled: GET ${url}`);
      }
      return { success: false, error: 'Request cancelled', cancelled: true };
    }
    
    return { success: false, error: getErrorMessage(error) };
  }
}

/**
 * Generic POST request handler with cancellation
 * @param url - The API endpoint URL
 * @param data - Request body data
 * @param token - Optional authentication token
 * @param signal - Optional AbortSignal for cancellation
 * @param requestId - Optional custom request ID for tracking
 */
export async function post<T>(
  url: string,
  data: unknown,
  token?: string,
  signal?: AbortSignal,
  requestId?: string
): Promise<ApiResponse<T>> {
  let reqId = requestId;
  
  try {
    // Setup cancellation if not provided
    const finalSignal = signal || (() => {
      const { requestId: id, signal: sig } = createCancellableRequest(reqId);
      reqId = id;
      return sig;
    })();

    const config = {
      ...(token ? { headers: createAuthHeader(token) } : {}),
      signal: finalSignal,
    };
    const response = await apiClient.post<T>(url, data, config);
    
    // Cleanup request tracking
    if (reqId) cleanupRequest(reqId);
    
    return { success: true, data: response.data };
  } catch (error) {
    // Cleanup request tracking
    if (reqId) cleanupRequest(reqId);
    
    // Check if request was cancelled
    if (error instanceof AxiosError && error.code === 'ERR_CANCELED') {
      if (process.env.NODE_ENV === 'development') {
        console.log(`🚫 Request Cancelled: POST ${url}`);
      }
      return { success: false, error: 'Request cancelled', cancelled: true };
    }
    
    return { success: false, error: getErrorMessage(error) };
  }
}

/**
 * Generic PUT request handler with cancellation
 * @param url - The API endpoint URL
 * @param data - Request body data
 * @param token - Optional authentication token
 * @param signal - Optional AbortSignal for cancellation
 * @param requestId - Optional custom request ID for tracking
 */
export async function put<T>(
  url: string,
  data: unknown,
  token?: string,
  signal?: AbortSignal,
  requestId?: string
): Promise<ApiResponse<T>> {
  let reqId = requestId;
  
  try {
    // Setup cancellation if not provided
    const finalSignal = signal || (() => {
      const { requestId: id, signal: sig } = createCancellableRequest(reqId);
      reqId = id;
      return sig;
    })();

    const config = {
      ...(token ? { headers: createAuthHeader(token) } : {}),
      signal: finalSignal,
    };
    const response = await apiClient.put<T>(url, data, config);
    
    // Cleanup request tracking
    if (reqId) cleanupRequest(reqId);
    
    return { success: true, data: response.data };
  } catch (error) {
    // Cleanup request tracking
    if (reqId) cleanupRequest(reqId);
    
    // Check if request was cancelled
    if (error instanceof AxiosError && error.code === 'ERR_CANCELED') {
      if (process.env.NODE_ENV === 'development') {
        console.log(`🚫 Request Cancelled: PUT ${url}`);
      }
      return { success: false, error: 'Request cancelled', cancelled: true };
    }
    
    return { success: false, error: getErrorMessage(error) };
  }
}

/**
 * Generic DELETE request handler with cancellation
 * @param url - The API endpoint URL
 * @param token - Optional authentication token
 * @param data - Optional request body data
 * @param signal - Optional AbortSignal for cancellation
 * @param requestId - Optional custom request ID for tracking
 */
export async function del<T>(
  url: string,
  token?: string,
  data?: unknown,
  signal?: AbortSignal,
  requestId?: string
): Promise<ApiResponse<T>> {
  let reqId = requestId;
  
  try {
    // Setup cancellation if not provided
    const finalSignal = signal || (() => {
      const { requestId: id, signal: sig } = createCancellableRequest(reqId);
      reqId = id;
      return sig;
    })();

    const config = {
      ...(token ? { headers: createAuthHeader(token) } : {}),
      data,
      signal: finalSignal,
    };
    const response = await apiClient.delete<T>(url, config);
    
    // Cleanup request tracking
    if (reqId) cleanupRequest(reqId);
    
    return { success: true, data: response.data };
  } catch (error) {
    // Cleanup request tracking
    if (reqId) cleanupRequest(reqId);
    
    // Check if request was cancelled
    if (error instanceof AxiosError && error.code === 'ERR_CANCELED') {
      if (process.env.NODE_ENV === 'development') {
        console.log(`🚫 Request Cancelled: DELETE ${url}`);
      }
      return { success: false, error: 'Request cancelled', cancelled: true };
    }
    
    return { success: false, error: getErrorMessage(error) };
  }
}

// Export the axios instance for advanced usage
export default apiClient;
