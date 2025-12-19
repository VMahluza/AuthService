# ? CORS Configuration - Fixed

## Problem Solved

**Error:** `Response to preflight request doesn't pass access control check: Redirect is not allowed for a preflight request`

**Root Cause:**
- HTTPS redirection middleware was intercepting OPTIONS preflight requests
- CORS middleware was missing entirely

---

## ?? What Was Fixed

### 1. Added CORS Policy
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

**Allowed Origins:**
- `http://localhost:3000` - Create React App default
- `http://localhost:5173` - Vite default

### 2. Fixed Middleware Order

**Critical:** CORS must come BEFORE HTTPS redirection!

```csharp
app.UseCors("AllowReactApp");           // ? First
app.UseHttpsRedirection();               // ? After CORS (dev only)
app.UseAuthentication();
app.UseAuthorization();
```

### 3. Disabled HTTPS Redirect in Development

Preflight requests don't follow redirects, so we disabled HTTPS redirection in development:

```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
```

---

## ?? Testing CORS

### From React App (Axios)

```typescript
import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5102/api',
  withCredentials: true, // Important for cookies/auth
});

// Test request
const response = await api.post('/auth/forgot-password', {
  email: 'user@example.com'
});
```

### From React App (Fetch)

```typescript
const response = await fetch('http://localhost:5102/api/auth/forgot-password', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  credentials: 'include', // Include cookies
  body: JSON.stringify({ email: 'user@example.com' })
});
```

### Expected Preflight Request

```http
OPTIONS http://localhost:5102/api/auth/forgot-password
Origin: http://localhost:3000
Access-Control-Request-Method: POST
Access-Control-Request-Headers: content-type
```

### Expected Preflight Response

```http
HTTP/1.1 204 No Content
Access-Control-Allow-Origin: http://localhost:3000
Access-Control-Allow-Methods: POST
Access-Control-Allow-Headers: content-type
Access-Control-Allow-Credentials: true
```

---

## ?? Production Configuration

### Option A: Environment-Specific Origins

**appsettings.Production.json:**
```json
{
  "AllowedOrigins": [
    "https://yourdomain.com",
    "https://www.yourdomain.com"
  ]
}
```

**Program.cs:**
```csharp
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

### Option B: Multiple Policies

```csharp
builder.Services.AddCors(options =>
{
    // Development policy
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    // Production policy
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://yourdomain.com")
              .AllowAnyHeader()
              .WithMethods("GET", "POST", "PUT", "DELETE")
              .AllowCredentials();
    });
});

// Use appropriate policy
var corsPolicy = app.Environment.IsDevelopment() ? "Development" : "Production";
app.UseCors(corsPolicy);
```

---

## ?? Security Considerations

### ? DO:
- Specify exact origins (not wildcards with credentials)
- Use HTTPS in production
- Limit methods if possible: `.WithMethods("GET", "POST")`
- Limit headers if possible: `.WithHeaders("Content-Type", "Authorization")`

### ? DON'T:
```csharp
// NEVER do this in production:
policy.AllowAnyOrigin()      // ? Security risk
      .AllowCredentials();   // ? Not allowed with AllowAnyOrigin
```

### Valid Wildcard Pattern (without credentials)
```csharp
// Only if you don't need cookies/auth
policy.AllowAnyOrigin()
      .AllowAnyHeader()
      .AllowAnyMethod();
// NOTE: Cannot use .AllowCredentials() with this
```

---

## ?? Complete Middleware Order Reference

**Correct Order:**
```csharp
app.UseCors("AllowReactApp");            // 1. CORS
app.UseHttpsRedirection();               // 2. HTTPS redirect
app.UseRouting();                        // 3. Routing (implicit)
app.UseAuthentication();                 // 4. Authentication
app.UseMiddleware<JwtRevocationMiddleware>(); // 5. Custom auth
app.UseAuthorization();                  // 6. Authorization
app.MapControllers();                    // 7. Endpoints
```

---

## ??? Troubleshooting

### Still Getting CORS Errors?

#### 1. Check Browser Console
Look for specific CORS error messages:
- `Access-Control-Allow-Origin` header missing
- `Access-Control-Allow-Credentials` mismatch
- Redirect during preflight

#### 2. Check Network Tab
- Is the OPTIONS request succeeding (204)?
- Are CORS headers present in the response?

#### 3. Verify API is Using HTTP in Development
Your React app is calling `http://localhost:5102`, not `https://`.

#### 4. Clear Browser Cache
Sometimes browsers cache preflight responses:
```bash
# Chrome DevTools
Right-click Refresh ? Empty Cache and Hard Reload
```

#### 5. Check API Logs
Enable detailed logging:
```csharp
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});
```

---

## ?? Quick Test Script

**PowerShell:**
```powershell
# Test preflight request
curl.exe -X OPTIONS http://localhost:5102/api/auth/forgot-password `
  -H "Origin: http://localhost:3000" `
  -H "Access-Control-Request-Method: POST" `
  -H "Access-Control-Request-Headers: content-type" `
  -v
```

**Expected Output:**
```
< HTTP/1.1 204 No Content
< Access-Control-Allow-Origin: http://localhost:3000
< Access-Control-Allow-Methods: POST
< Access-Control-Allow-Headers: content-type
< Access-Control-Allow-Credentials: true
```

---

## ?? Summary

? CORS policy added with allowed origins  
? Middleware order fixed (CORS before HTTPS redirect)  
? HTTPS redirect disabled in development  
? `AllowCredentials()` enabled for JWT authentication  
? API builds successfully

**Your React app should now connect without CORS errors!**

---

## ?? Next Steps

1. Restart your API:
   ```bash
   dotnet run --project AuthService.API
   ```

2. Test from React app:
   ```typescript
   await axios.post('http://localhost:5102/api/auth/forgot-password', {
     email: 'test@example.com'
   });
   ```

3. Verify no CORS errors in browser console

4. For production, update `AllowedOrigins` to your actual domain
