import { AxiosError } from 'axios';
import { apiClient } from './config';
import { getErrorMessage, createAuthHeader } from './utils';
import { generateCacheKey, getFromCache, setCache } from './cache';
import { createCancellableRequest, cleanupRequest } from './cancellation';
import type { ApiResponse } from './types';

/**
 * HTTP Methods Module
 * Single Responsibility: Generic HTTP request methods (GET, POST, PUT, DELETE)
 */

/**
 * Generic GET request handler with caching and cancellation
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
