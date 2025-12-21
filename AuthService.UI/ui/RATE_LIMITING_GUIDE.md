# Rate Limiting Handling - Quick Reference

## What It Does

Automatically detects and handles 429 (Too Many Requests) responses with intelligent retry logic.

## How It Works

```
Request  429 Response  Wait (Retry-After or Backoff)  Retry  Success/Failure
```

## Retry Strategy

### With Retry-After Header
Server tells us exactly when to retry:
```
Attempt 1: 429  Wait 5s (from Retry-After header)  Retry
Attempt 2: 429  Wait 10s (from Retry-After header)  Retry
Attempt 3: 429  Wait 15s (from Retry-After header)  Retry
Max retries reached  Return error
```

### Without Retry-After (Exponential Backoff)
Smart automatic delays:
```
Attempt 1: 429  Wait 1s (2^0)  Retry
Attempt 2: 429  Wait 2s (2^1)  Retry
Attempt 3: 429  Wait 4s (2^2)  Retry
Max retries reached  Return error
```

## Console Output Examples

### Successful Retry
```bash
 API Error: {
  method: "GET",
  url: "/users",
  status: 429,
  statusText: "Too Many Requests",
  message: "Rate limit exceeded"
}
 Rate Limit: Waiting 5000ms before retry (attempt 1/3)
 Rate Limit: Retrying request to /users
 API Response: { status: 200, data: [...], duration: '234ms' }
```

### Multiple Retries
```bash
 API Error: { status: 429, ... }
 Rate Limit: Waiting 1000ms before retry (attempt 1/3)
 Rate Limit: Retrying request to /users

 API Error: { status: 429, ... }
 Rate Limit: Waiting 2000ms before retry (attempt 2/3)
 Rate Limit: Retrying request to /users

 API Error: { status: 429, ... }
 Rate Limit: Waiting 4000ms before retry (attempt 3/3)
 Rate Limit: Retrying request to /users

 API Response: { status: 200, ... }
```

### Max Retries Reached
```bash
 API Error: { status: 429, ... }
 Rate Limit: Waiting 1000ms before retry (attempt 1/3)
 Rate Limit: Retrying request to /users

 API Error: { status: 429, ... }
 Rate Limit: Waiting 2000ms before retry (attempt 2/3)
 Rate Limit: Retrying request to /users

 API Error: { status: 429, ... }
 Rate Limit: Waiting 4000ms before retry (attempt 3/3)
 Rate Limit: Retrying request to /users

 API Error: { status: 429, ... }
 Rate Limit: Max retries reached, giving up
```

## Configuration

| Setting | Default | Description |
|---------|---------|-------------|
| Max Retries | 3 | Number of retry attempts |
| Max Delay | 30s | Maximum wait time per retry |
| Backoff Base | 2 | Exponential growth factor (2^n) |
| Retry-After Priority | Yes | Always use server value if present |

## Usage

### No Code Changes Needed!
```typescript
// Automatically handled for all requests
const result = await get<User[]>('/users', token);
const result = await post<User>('/users', userData, token);
const result = await put<User>('/users/1', userData, token);
const result = await del('/users/1', token);
```

### User Feedback Pattern
```typescript
export async function getUsers(token: string) {
  setLoading(true);
  setMessage('Loading users...');
  
  const result = await get<User[]>('/users', token);
  
  if (!result.success) {
    if (result.error?.includes('Too Many Requests')) {
      setMessage('Server is busy. Please try again in a moment.');
    } else {
      setMessage('Failed to load users');
    }
  }
  
  setLoading(false);
  return result;
}
```

## Benefits

 **Zero Configuration**: Works automatically  
 **Server-Friendly**: Respects Retry-After headers  
 **Smart Backoff**: Reduces server load  
 **Transparent**: User sees normal loading state  
 **Production-Ready**: No dev-only features  
 **Resilient**: Handles burst traffic gracefully

## Emoji Legend

| Emoji | Meaning |
|-------|---------|
|  | Waiting before retry |
|  | Retrying request |
|  | Success after retry |
|  | Error or max retries |

## Best Practices

###  DO
- Let the feature handle rate limits automatically
- Show loading states during retries
- Inform users if max retries reached
- Monitor console in development
- Batch requests wisely

###  DON'T
- Implement custom retry logic
- Ignore max retry errors
- Make rapid sequential requests
- Disable the feature for important requests

## Monitoring

### Development
Watch for patterns indicating need to optimize:
```bash
# Too many rate limits?
 Rate Limit: Waiting 1000ms before retry (attempt 1/3)
 Rate Limit: Waiting 1000ms before retry (attempt 1/3)
 Rate Limit: Waiting 1000ms before retry (attempt 1/3)
#  Consider batching or throttling requests
```

### Production
- Log 429 responses to analytics
- Alert on repeated max-retry failures
- Track retry success rates
- Optimize request patterns based on data

## Technical Details

### Retry-After Header Parsing
```typescript
// Seconds format
Retry-After: 60          Wait 60,000ms

// Date format
Retry-After: Wed, 21 Dec 2025 12:00:00 GMT
 Calculate milliseconds until date
```

### Exponential Backoff Formula
```typescript
delay = 2^retryCount * 1000  // milliseconds
delay = Math.min(delay, 30000)  // cap at 30s

// Results:
// Retry 0: 1s (2^0 * 1000)
// Retry 1: 2s (2^1 * 1000)
// Retry 2: 4s (2^2 * 1000)
// Retry 3: 8s (2^3 * 1000)
```

### Max Wait Time
```typescript
// Worst case timeline:
Attempt 1: 0s + 1s wait = 1s
Attempt 2: 1s + 2s wait = 3s
Attempt 3: 3s + 4s wait = 7s
Max retries: 7s + error = ~7s total

// With Retry-After (capped at 30s each):
Attempt 1: 0s + 30s wait = 30s
Attempt 2: 30s + 30s wait = 60s
Attempt 3: 60s + 30s wait = 90s
Max retries: 90s + error = ~90s total
```

## Integration Example

### Component with Retry Feedback
```typescript
'use client';

import { useState } from 'react';
import { getUsers } from './actions';

export default function UsersPage() {
  const [loading, setLoading] = useState(false);
  const [retrying, setRetrying] = useState(false);
  
  const handleLoad = async () => {
    setLoading(true);
    
    // Monitor console for retry messages
    const result = await getUsers(token);
    
    if (!result.success) {
      alert('Unable to load users. Server may be busy.');
    }
    
    setLoading(false);
  };
  
  return (
    <div>
      {loading && <p>Loading users...</p>}
      {retrying && <p>Server busy, retrying...</p>}
      <button onClick={handleLoad}>Load Users</button>
    </div>
  );
}
```

---

**Result**: Automatic, intelligent rate limit handling with zero configuration! 
