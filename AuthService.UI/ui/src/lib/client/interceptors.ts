import { AxiosError, InternalAxiosRequestConfig } from 'axios';
import { apiClient } from './config';
import {
  refreshAccessToken,
  processQueue,
  getRefreshState,
  setRefreshState,
  addToFailedQueue,
} from './token-refresh';

/**
 * Interceptors Module
 * Single Responsibility: Configure request/response interceptors
 */

/**
 * Setup request interceptor
 * Handles: Performance tracking, development logging
 */
function setupRequestInterceptor(): void {
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
}

/**
 * Setup response interceptor
 * Handles: Success logging, error handling, token refresh, rate limiting
 */
function setupResponseInterceptor(): void {
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
        return handleRateLimiting(error, originalRequest);
      }

      // Token Refresh Handling (401 Unauthorized)
      if (error.response?.status === 401 && !originalRequest._retry) {
        return handleTokenRefresh(error, originalRequest);
      }

      // For other errors, just reject
      return Promise.reject(error);
    }
  );
}

/**
 * Handle rate limiting with exponential backoff
 */
async function handleRateLimiting(
  error: AxiosError,
  originalRequest: InternalAxiosRequestConfig & { _retryCount?: number }
): Promise<unknown> {
  const retryCount = originalRequest._retryCount || 0;
  const maxRetries = 3;

  if (retryCount < maxRetries) {
    // Get retry delay from Retry-After header or use exponential backoff
    const retryAfter = error.response?.headers['retry-after'];
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

/**
 * Handle token refresh on 401 errors
 */
async function handleTokenRefresh(
  error: AxiosError,
  originalRequest: InternalAxiosRequestConfig & { _retry?: boolean }
): Promise<unknown> {
  if (getRefreshState()) {
    // If already refreshing, queue this request
    return new Promise((resolve, reject) => {
      addToFailedQueue({ resolve, reject });
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
  setRefreshState(true);

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
    setRefreshState(false);
  }
}

/**
 * Initialize all interceptors
 */
export function setupInterceptors(): void {
  setupRequestInterceptor();
  setupResponseInterceptor();
}
