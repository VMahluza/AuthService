# Request Cancellation - Quick Reference

## What It Does

AbortController-based request cancellation to prevent memory leaks, handle component unmounts, and cancel outdated requests.

## How It Works

```
Create AbortController  Pass Signal  Make Request  Cancel (optional)  Auto Cleanup
```

## Basic Usage

### Automatic Tracking
```typescript
import { get, cancelAllRequests } from '@/lib/api-client';

// Request automatically tracked
const result = await get<User[]>('/users', token);

// Cancel all if needed
cancelAllRequests();
```

### Manual Control
```typescript
import { createCancellableRequest, get, cancelRequest } from '@/lib/api-client';

// Create with signal
const { requestId, signal } = createCancellableRequest();

// Pass signal to request
const result = await get<User[]>('/users', token, true, undefined, signal);

// Cancel specific request
cancelRequest(requestId);
```

### Custom Request ID
```typescript
const { requestId, signal } = createCancellableRequest('my-search');
await get<Results>('/search', token, false, undefined, signal, 'my-search');
cancelRequest('my-search');
```

## Cancellation Functions

| Function | Purpose | Returns |
|----------|---------|---------|
| `createCancellableRequest(id?)` | Create AbortController with tracking | `{ requestId, signal }` |
| `cancelRequest(requestId)` | Cancel specific request | `boolean` |
| `cancelAllRequests()` | Cancel all active requests | `number` (count) |
| `cancelRequestsByUrl(pattern)` | Cancel by URL pattern | `number` (count) |
| `getActiveRequestCount()` | Get active request count | `number` |

## React Patterns

### 1. Component Unmount Cleanup
```typescript
useEffect(() => {
  const { requestId, signal } = createCancellableRequest();
  
  const fetchData = async () => {
    const result = await get<User[]>('/users', token, true, undefined, signal);
    
    if (result.success && !result.cancelled) {
      setUsers(result.data || []);
    }
  };
  
  fetchData();
  
  // Cleanup on unmount
  return () => cancelRequest(requestId);
}, []);
```

### 2. Search with Debounce
```typescript
const [query, setQuery] = useState('');
const requestIdRef = useRef<string | null>(null);

useEffect(() => {
  // Cancel previous search
  if (requestIdRef.current) {
    cancelRequest(requestIdRef.current);
  }

  if (!query) return;

  const timeoutId = setTimeout(async () => {
    const { requestId, signal } = createCancellableRequest();
    requestIdRef.current = requestId;

    const result = await get<Results>(
      `/search?q=${query}`, 
      token, 
      false, 
      undefined, 
      signal
    );

    if (result.success && !result.cancelled) {
      setResults(result.data || []);
    }
  }, 300);

  return () => clearTimeout(timeoutId);
}, [query]);
```

### 3. Tab/Route Switching
```typescript
const pathname = usePathname();

useEffect(() => {
  // Cancel all requests on route change
  return () => cancelAllRequests();
}, [pathname]);
```

### 4. Cancel Before Submit
```typescript
const handleSubmit = async () => {
  // Cancel any pending GET requests
  cancelAllRequests();
  
  // Submit form
  const result = await post('/users', formData, token);
  
  if (result.success) {
    router.push('/users');
  }
};
```

### 5. Long Polling with Cancel
```typescript
const pollingIdRef = useRef<string | null>(null);

const startPolling = () => {
  const poll = async () => {
    const { requestId, signal } = createCancellableRequest('polling');
    pollingIdRef.current = requestId;
    
    const result = await get('/status', token, false, undefined, signal);
    
    if (!result.cancelled) {
      setTimeout(poll, 5000);
    }
  };
  
  poll();
};

const stopPolling = () => {
  if (pollingIdRef.current) {
    cancelRequest(pollingIdRef.current);
  }
};

// Start on mount, stop on unmount
useEffect(() => {
  startPolling();
  return stopPolling;
}, []);
```

## Console Output Examples

### Request Lifecycle
```bash
 Request Registered: req_1_1703174400000 (1 active)
 API Request: { method: 'GET', url: '/users', ... }
 Request Completed: req_1_1703174400000 (0 active)
 API Response: { status: 200, ... }
```

### Manual Cancellation
```bash
 Request Registered: req_2_1703174401000 (1 active)
 API Request: { method: 'GET', url: '/users', ... }
 Request Cancelled: req_2_1703174401000
 Request Cancelled: GET /users
```

### Bulk Cancellation
```bash
 Request Registered: req_3_1703174402000 (3 active)
 Request Registered: req_4_1703174402001 (4 active)
 Request Registered: req_5_1703174402002 (5 active)
 Request Cancelled: req_3_1703174402000
 Request Cancelled: req_4_1703174402001
 Request Cancelled: req_5_1703174402002
 All Requests Cancelled: 3 total
```

### Pattern Matching
```bash
 Request Registered: req_6_1703174403000 (3 active)
 Request Cancelled: req_6_1703174403000
 Requests Cancelled by Pattern: 2 matched
```

## API Response Structure

```typescript
interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
  cancelled?: boolean;  //  Indicates cancellation
}

// Check cancellation
const result = await get<User[]>('/users', token, true, undefined, signal);

if (result.cancelled) {
  console.log('Request was cancelled');
} else if (result.success) {
  console.log('Data:', result.data);
} else {
  console.error('Error:', result.error);
}
```

## Method Signatures

### GET
```typescript
get<T>(
  url: string,
  token?: string,
  useCache?: boolean,        // default: true
  cacheTTL?: number,         // default: 5 min
  signal?: AbortSignal,      // optional
  requestId?: string         // optional
)
```

### POST
```typescript
post<T>(
  url: string,
  data: unknown,
  token?: string,
  signal?: AbortSignal,      // optional
  requestId?: string         // optional
)
```

### PUT
```typescript
put<T>(
  url: string,
  data: unknown,
  token?: string,
  signal?: AbortSignal,      // optional
  requestId?: string         // optional
)
```

### DELETE
```typescript
del<T>(
  url: string,
  token?: string,
  data?: unknown,
  signal?: AbortSignal,      // optional
  requestId?: string         // optional
)
```

## Benefits

 **Memory Leak Prevention**: Auto-cancel on unmount  
 **Race Condition Fix**: Cancel outdated requests  
 **Performance**: Reduce unnecessary traffic  
 **User Control**: Cancel long-running operations  
 **Clean Code**: Centralized cancellation  
 **Type Safe**: Full TypeScript support

## Best Practices

###  DO
- Always cleanup in useEffect return
- Cancel previous search when user types
- Use custom IDs for specific scenarios
- Check `cancelled` flag before updating state
- Cancel all requests on navigation

###  DON'T
- Ignore the `cancelled` flag
- Reuse AbortSignal across requests
- Forget to cleanup on unmount
- Cancel critical mutation requests
- Use cancellation for error handling

## Common Patterns

### Request Manager Class
```typescript
class RequestManager {
  private requests = new Map<string, string>();
  
  async fetch<T>(key: string, url: string, token?: string) {
    // Cancel previous
    if (this.requests.has(key)) {
      cancelRequest(this.requests.get(key)!);
    }
    
    // Create new
    const { requestId, signal } = createCancellableRequest(key);
    this.requests.set(key, requestId);
    
    const result = await get<T>(url, token, true, undefined, signal);
    this.requests.delete(key);
    
    return result;
  }
  
  cancelAll() {
    this.requests.forEach(id => cancelRequest(id));
    this.requests.clear();
  }
}
```

### Timeout Wrapper
```typescript
async function getWithTimeout<T>(
  url: string,
  token: string,
  timeout: number = 5000
): Promise<ApiResponse<T>> {
  const { requestId, signal } = createCancellableRequest();
  
  const timeoutId = setTimeout(() => {
    cancelRequest(requestId);
  }, timeout);
  
  const result = await get<T>(url, token, true, undefined, signal);
  clearTimeout(timeoutId);
  
  return result;
}
```

## Troubleshooting

### Memory Leaks
```typescript
//  Wrong - no cleanup
useEffect(() => {
  get('/users', token);
}, []);

//  Correct - cleanup on unmount
useEffect(() => {
  const { requestId, signal } = createCancellableRequest();
  get('/users', token, true, undefined, signal);
  return () => cancelRequest(requestId);
}, []);
```

### Race Conditions
```typescript
//  Wrong - old results may arrive last
const handleSearch = async (query: string) => {
  const result = await get(`/search?q=${query}`, token, false);
  setResults(result.data);
};

//  Correct - cancel previous
const handleSearch = async (query: string) => {
  if (requestIdRef.current) {
    cancelRequest(requestIdRef.current);
  }
  
  const { requestId, signal } = createCancellableRequest();
  requestIdRef.current = requestId;
  
  const result = await get(`/search?q=${query}`, token, false, undefined, signal);
  
  if (!result.cancelled) {
    setResults(result.data);
  }
};
```

## Performance Impact

- **Registration**: ~0.1ms overhead
- **Cancellation**: Instant (abort)
- **Memory**: ~100 bytes per request
- **Cleanup**: Automatic

## Emoji Legend

| Emoji | Meaning |
|-------|---------|
|  | Request registered |
|  | Request cancelled |
|  | Request completed |

---

**Result**: Complete request cancellation with memory leak prevention! 
