import type { CancellableRequest } from './types';

/**
 * Request Cancellation Module
 * Single Responsibility: Handle AbortController-based request cancellation
 */

const activeRequests = new Map<string, AbortController>();
let requestIdCounter = 0;

/**
 * Generate unique request ID
 */
function generateRequestId(): string {
  return `req_${++requestIdCounter}_${Date.now()}`;
}

/**
 * Create an AbortController and register it
 */
export function createCancellableRequest(requestId?: string): CancellableRequest {
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
export function cleanupRequest(requestId: string): void {
  activeRequests.delete(requestId);
  
  if (process.env.NODE_ENV === 'development') {
    console.log(`✅ Request Completed: ${requestId} (${activeRequests.size} active)`);
  }
}
