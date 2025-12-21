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
    // Get refresh token (client-side only)
    const refreshToken = getRefreshToken();
    if (!refreshToken) {
      return null;
    }

    const response = await axios.post(`${BACKEND_BASE_URL}/auth/refresh`, {
      refreshToken,
    });

    const { accessToken, refreshToken: newRefreshToken } = response.data;

    // Store new tokens
    setTokens(accessToken, newRefreshToken || refreshToken);

    return accessToken;
  } catch {
    // Clear tokens if refresh fails
    clearTokens();
    return null;
  }
}

// Request interceptor to add auth token
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // Token will be added per request when needed
    // This can be enhanced to automatically get from session/cookies
    return config;
  },
  (error: AxiosError) => {
    return Promise.reject(error);
  }
);

// Response interceptor for global error handling and automatic token refresh
apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };

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
 * Generic GET request handler
 */
export async function get<T>(
  url: string,
  token?: string
): Promise<ApiResponse<T>> {
  try {
    const config = token ? { headers: createAuthHeader(token) } : {};
    const response = await apiClient.get<T>(url, config);
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
