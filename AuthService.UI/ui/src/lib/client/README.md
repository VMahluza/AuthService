# API Client Architecture

## Overview

The API client has been refactored following **SOLID principles** and **Separation of Concerns** for maximum maintainability and testability.

## Architecture Principles

###  Single Responsibility Principle (SRP)
Each module has one specific responsibility:
- `config.ts` - Axios instance configuration
- `types.ts` - TypeScript interfaces and types
- `utils.ts` - Utility functions (error handling, auth headers)
- `token-refresh.ts` - Token refresh logic and request queueing
- `cache.ts` - GET request caching management
- `cancellation.ts` - AbortController-based request cancellation
- `interceptors.ts` - Request/response interceptors
- `http-methods.ts` - Generic HTTP methods (GET, POST, PUT, DELETE)
- `index.ts` - Public API exports

###  Separation of Concerns
Each concern is isolated in its own module:
- **Configuration**  `config.ts`
- **Type Definitions**  `types.ts`
- **Business Logic**  `token-refresh.ts`, `cache.ts`, `cancellation.ts`
- **Request Handling**  `interceptors.ts`, `http-methods.ts`
- **Utilities**  `utils.ts`
- **Public API**  `index.ts`

###  DRY (Don't Repeat Yourself)
- No code duplication
- Shared utilities in dedicated modules
- Single source of truth for each feature

###  Open/Closed Principle
- Open for extension (add new features)
- Closed for modification (existing code unchanged)

## File Structure

```
src/lib/client/
 index.ts              # Public API exports
 config.ts             # Axios instance
 types.ts              # TypeScript interfaces
 utils.ts              # Utility functions
 token-refresh.ts      # Token refresh logic
 cache.ts              # Cache management
 cancellation.ts       # Request cancellation
 interceptors.ts       # Request/response interceptors
 http-methods.ts       # HTTP methods (GET, POST, PUT, DELETE)
```

## Module Responsibilities

### 1. `config.ts`
**Responsibility:** Axios instance configuration

```typescript
export const apiClient: AxiosInstance = axios.create({
  baseURL: BACKEND_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
  timeout: 10000,
});
```

### 2. `types.ts`
**Responsibility:** TypeScript interfaces and types

```typescript
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
  cancelled?: boolean;
}

export interface CacheEntry<T> { /* ... */ }
export interface CancellableRequest { /* ... */ }
export interface FailedQueueItem { /* ... */ }
export interface CacheStats { /* ... */ }
```

### 3. `utils.ts`
**Responsibility:** Utility functions

```typescript
export function getErrorMessage(error: unknown): string { /* ... */ }
export function createAuthHeader(token: string) { /* ... */ }
```

### 4. `token-refresh.ts`
**Responsibility:** Token refresh logic and request queueing

```typescript
export async function refreshAccessToken(): Promise<string | null> { /* ... */ }
export function processQueue(error: AxiosError | null, token: string | null): void { /* ... */ }
export function getRefreshState(): boolean { /* ... */ }
export function setRefreshState(state: boolean): void { /* ... */ }
export function addToFailedQueue(item: FailedQueueItem): void { /* ... */ }
```

### 5. `cache.ts`
**Responsibility:** GET request caching

```typescript
export function generateCacheKey(url: string, token?: string): string { /* ... */ }
export function getFromCache<T>(key: string): T | null { /* ... */ }
export function setCache<T>(key: string, data: T, ttl?: number): void { /* ... */ }
export function clearCache(key?: string): void { /* ... */ }
export function getCacheStats(): CacheStats { /* ... */ }
```

### 6. `cancellation.ts`
**Responsibility:** Request cancellation management

```typescript
export function createCancellableRequest(requestId?: string): CancellableRequest { /* ... */ }
export function cancelRequest(requestId: string): boolean { /* ... */ }
export function cancelAllRequests(): number { /* ... */ }
export function cancelRequestsByUrl(pattern: string | RegExp): number { /* ... */ }
export function getActiveRequestCount(): number { /* ... */ }
export function cleanupRequest(requestId: string): void { /* ... */ }
```

### 7. `interceptors.ts`
**Responsibility:** Configure axios interceptors

```typescript
export function setupInterceptors(): void { /* ... */ }
// - Request interceptor: logging, performance tracking
// - Response interceptor: logging, error handling, token refresh, rate limiting
```

### 8. `http-methods.ts`
**Responsibility:** Generic HTTP methods

```typescript
export async function get<T>(url, token?, useCache?, cacheTTL?, signal?, requestId?): Promise<ApiResponse<T>> { /* ... */ }
export async function post<T>(url, data, token?, signal?, requestId?): Promise<ApiResponse<T>> { /* ... */ }
export async function put<T>(url, data, token?, signal?, requestId?): Promise<ApiResponse<T>> { /* ... */ }
export async function del<T>(url, token?, data?, signal?, requestId?): Promise<ApiResponse<T>> { /* ... */ }
```

### 9. `index.ts`
**Responsibility:** Public API exports

```typescript
// Initialize interceptors on import
import { setupInterceptors } from './interceptors';
setupInterceptors();

// Export all public APIs
export { get, post, put, del } from './http-methods';
export { clearCache, getCacheStats } from './cache';
export { createCancellableRequest, cancelRequest, /* ... */ } from './cancellation';
export { getErrorMessage, createAuthHeader } from './utils';
export type { ApiResponse, CacheStats } from './types';
export { apiClient as default } from './config';
```

## Usage

### Import from `@/lib/client`

```typescript
// HTTP methods
import { get, post, put, del } from '@/lib/client';

// Cache utilities
import { clearCache, getCacheStats } from '@/lib/client';

// Cancellation
import { createCancellableRequest, cancelRequest, cancelAllRequests } from '@/lib/client';

// Types
import type { ApiResponse } from '@/lib/client';

// Utilities
import { getErrorMessage, createAuthHeader } from '@/lib/client';

// Advanced: Direct axios instance
import apiClient from '@/lib/client';
```

### Example: Simple GET Request

```typescript
import { get, type ApiResponse } from '@/lib/client';

interface User {
  id: string;
  name: string;
}

export async function getUsers(token: string): Promise<ApiResponse<User[]>> {
  return get<User[]>('/users', token);
}
```

### Example: POST with Cancellation

```typescript
import { post, createCancellableRequest, cancelRequest, type ApiResponse } from '@/lib/client';

const { requestId, signal } = createCancellableRequest('create-user');

const result = await post<User>('/users', userData, token, signal, requestId);

// Cancel if needed
cancelRequest(requestId);
```

## Benefits

###  Maintainability
- Easy to locate specific functionality
- Changes isolated to single modules
- Clear module boundaries

###  Testability
- Each module can be unit tested independently
- Mock specific modules without affecting others
- Clear dependencies

###  Readability
- Descriptive file names
- Small, focused modules
- Clear module responsibilities

###  Extensibility
- Add new features without modifying existing code
- Easy to add new modules
- Minimal impact on existing functionality

###  Scalability
- Structure supports growth
- Easy to add team members
- Clear code ownership

## Comparison: Before vs After

### Before (Monolithic)
```
src/lib/
 api-client.ts (750+ lines)
     Axios config
     Token refresh
     Interceptors
     Cache
     Cancellation
     HTTP methods
     Types
     Utils
```

**Problems:**
-  Single file with multiple responsibilities
-  Hard to navigate and maintain
-  Difficult to test specific features
-  High coupling between concerns

### After (Modular)
```
src/lib/client/
 index.ts (30 lines) - Public API
 config.ts (12 lines) - Axios config
 types.ts (48 lines) - Types
 utils.ts (27 lines) - Utilities
 token-refresh.ts (91 lines) - Token refresh
 cache.ts (102 lines) - Cache
 cancellation.ts (123 lines) - Cancellation
 interceptors.ts (221 lines) - Interceptors
 http-methods.ts (201 lines) - HTTP methods
```

**Benefits:**
-  Clear separation of concerns
-  Easy to navigate and maintain
-  Simple to test individual features
-  Low coupling, high cohesion

## Testing Strategy

Each module can be tested independently:

```typescript
// test/cache.test.ts
import { setCache, getFromCache, clearCache } from '@/lib/client/cache';

describe('Cache Module', () => {
  test('should cache and retrieve data', () => {
    setCache('test-key', { id: 1 });
    expect(getFromCache('test-key')).toEqual({ id: 1 });
  });
});
```

## Migration Guide

### Old Import
```typescript
import { get, post } from '@/lib/api-client';
```

### New Import
```typescript
import { get, post } from '@/lib/client';
```

**Note:** All imports have been automatically updated!

## SOLID Compliance

###  Single Responsibility Principle
Each module has one reason to change.

###  Open/Closed Principle
Open for extension, closed for modification.

###  Liskov Substitution Principle
Modules can be replaced with implementations of same interface.

###  Interface Segregation Principle
Clients only depend on methods they use.

###  Dependency Inversion Principle
Depend on abstractions (interfaces), not concretions.

---

**Result:** Clean, maintainable, SOLID-compliant API client architecture! 
