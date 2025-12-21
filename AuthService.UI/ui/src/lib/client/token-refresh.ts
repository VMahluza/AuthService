import axios, { AxiosError } from 'axios';
import { BACKEND_BASE_URL } from '../constants';
import { getRefreshToken, setTokens, clearTokens } from '../token-utils';
import type { FailedQueueItem } from './types';

/**
 * Token Refresh Module
 * Single Responsibility: Handle token refresh logic and request queueing
 */

// Token refresh state
let isRefreshing = false;
let failedQueue: FailedQueueItem[] = [];

/**
 * Process queued requests after token refresh
 */
export function processQueue(error: AxiosError | null, token: string | null = null): void {
  failedQueue.forEach((promise) => {
    if (error) {
      promise.reject(error);
    } else if (token) {
      promise.resolve(token);
    }
  });
  failedQueue = [];
}

/**
 * Get refresh state
 */
export function getRefreshState(): boolean {
  return isRefreshing;
}

/**
 * Set refresh state
 */
export function setRefreshState(state: boolean): void {
  isRefreshing = state;
}

/**
 * Add request to failed queue
 */
export function addToFailedQueue(item: FailedQueueItem): void {
  failedQueue.push(item);
}

/**
 * Refresh the access token using the refresh token from localStorage
 */
export async function refreshAccessToken(): Promise<string | null> {
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
