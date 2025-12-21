# Request/Response Logging - Quick Reference

## Console Output Examples

### Successful Request
```
 API Request: {
  method: "GET",
  url: "/groups",
  baseURL: "http://localhost:5102/api",
  data: undefined,
  params: undefined,
  headers: {
    "Content-Type": "application/json",
    Authorization: "***HIDDEN***"
  }
}

 API Response: {
  method: "GET",
  url: "/groups",
  status: 200,
  statusText: "OK",
  data: [{ id: "1", name: "Admin", description: "..." }],
  duration: "245ms"
}
```

### Failed Request (401 with Auto-Refresh)
```
 API Request: {
  method: "GET",
  url: "/users",
  ...
}

 API Error: {
  method: "GET",
  url: "/users",
  status: 401,
  statusText: "Unauthorized",
  message: "Token expired"
}

 Token Refresh: Attempting to refresh access token...
 Token Refresh: Successfully refreshed access token

 API Request: {
  method: "GET",
  url: "/users",
  ...
}

 API Response: {
  method: "GET",
  url: "/users",
  status: 200,
  duration: "678ms"
}
```

### Error with No Refresh Token
```
 API Request: { ... }

 API Error: {
  status: 401,
  message: "Unauthorized"
}

 Token Refresh: Attempting to refresh access token...
 Token Refresh: No refresh token found
 Token Refresh: Failed to refresh token, clearing storage
 Redirecting to login page
```

### POST Request with Data
```
 API Request: {
  method: "POST",
  url: "/groups",
  data: {
    name: "New Group",
    description: "Group description"
  },
  headers: {
    "Content-Type": "application/json",
    Authorization: "***HIDDEN***"
  }
}

 API Response: {
  method: "POST",
  url: "/groups",
  status: 201,
  statusText: "Created",
  data: { id: "123", name: "New Group", ... },
  duration: "412ms"
}
```

## Emoji Legend

| Emoji | Meaning |
|-------|---------|
|  | Request sent |
|  | Success response |
|  | Error response |
|  | Token refresh in progress |
|  | Warning/No refresh token |

## Configuration

**Development Mode (Logging ON):**
```env
NODE_ENV=development
```

**Production Mode (Logging OFF):**
```env
NODE_ENV=production
```

## Features

 Color-coded console output (emojis)
 Request/Response pairing
 Performance timing (duration)
 Token security (Authorization hidden)
 Automatic refresh tracking
 Error details included
 Zero configuration needed

## Browser DevTools Tips

### Filter Logs
- Chrome: Filter by "API Request" or "API Response"
- Firefox: Use the search bar in console
- Edge: Filter by log level

### Save Logs
```javascript
// Copy all API logs
copy(console.history.filter(log => 
  log.includes('API') || log.includes('Token Refresh')
))
```

### Monitor Performance
```javascript
// Track slow requests (>500ms)
const slowRequests = console.history
  .filter(log => log.duration && parseInt(log.duration) > 500)
  .map(log => ({ url: log.url, duration: log.duration }));
```

---

**Result**: Complete visibility into API calls during development! 
