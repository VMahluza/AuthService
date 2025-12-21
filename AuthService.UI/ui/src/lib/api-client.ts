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
    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };

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
  const now = Date.now();
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
 * Generic API response type
 */
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
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
 * Generic GET request handler with caching
 * @param url - The API endpoint URL
 * @param token - Optional authentication token
 * @param useCache - Whether to use cache (default: true)
 * @param cacheTTL - Cache time-to-live in milliseconds (default: 5 minutes)
 */
export async function get<T>(
  url: string,
  token?: string,
  useCache: boolean = true,
  cacheTTL?: number
): Promise<ApiResponse<T>> {
  try {
    // Check cache first if enabled
    if (useCache) {
      const cacheKey = generateCacheKey(url, token);
      const cachedData = getFromCache<T>(cacheKey);
      
      if (cachedData !== null) {
        return { success: true, data: cachedData };
      }
    }

    // Make actual request
    const config = token ? { headers: createAuthHeader(token) } : {};
    const response = await apiClient.get<T>(url, config);
    
    // Store in cache if enabled
    if (useCache) {
      const cacheKey = generateCacheKey(url, token);
      setCache(cacheKey, response.data, cacheTTL);
    }
    
    return { success: true, data: response.data };
  } catch (error) {
    return { success: false, error: getErrorMessage(error) };
  }
}

/**
 * Generic POST request handler
 */
export async function post<T>(
  url: string,
  data: unknown,
  token?: string
): Promise<ApiResponse<T>> {
  try {
    const config = token ? { headers: createAuthHeader(token) } : {};
    const response = await apiClient.post<T>(url, data, config);
    return { success: true, data: response.data };
  } catch (error) {
    return { success: false, error: getErrorMessage(error) };
  }
}

/**
 * Generic PUT request handler
 */
export async function put<T>(
  url: string,
  data: unknown,
  token?: string
): Promise<ApiResponse<T>> {
  try {
    const config = token ? { headers: createAuthHeader(token) } : {};
    const response = await apiClient.put<T>(url, data, config);
    return { success: true, data: response.data };
  } catch (error) {
    return { success: false, error: getErrorMessage(error) };
  }
}

/**
 * Generic DELETE request handler
 */
export async function del<T>(
  url: string,
  token?: string,
  data?: unknown
): Promise<ApiResponse<T>> {
  try {
    const config = token
      ? { headers: createAuthHeader(token), data }
      : { data };
    const response = await apiClient.delete<T>(url, config);
    return { success: true, data: response.data };
  } catch (error) {
    return { success: false, error: getErrorMessage(error) };
  }
}

// Export the axios instance for advanced usage
export default apiClient;
