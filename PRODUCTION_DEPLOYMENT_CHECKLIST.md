# ?? CRITICAL: Production Deployment Checklist

## Before Deploying to Production

### 1. ?? **SET JWT SECRET KEY** (CRITICAL)

**DO NOT use appsettings.json secret in production!**

#### Option A: Environment Variables
```bash
# Linux/Mac
export JWT_SECRET="your-secure-256-bit-secret-minimum-32-characters-generated-randomly-abc123"

# Windows PowerShell
$env:JWT_SECRET="your-secure-256-bit-secret-minimum-32-characters-generated-randomly-abc123"

# Docker
docker run -e JWT_SECRET="your-secret" your-image
```

#### Option B: Azure Key Vault
```csharp
// In Program.cs (before building)
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());

// Reference in appsettings.json
"JwtSettings": {
  "Secret": "@Microsoft.KeyVault(SecretUri=https://yourkeyvault.vault.azure.net/secrets/jwt-secret)"
}
```

#### Option C: AWS Secrets Manager
```bash
aws secretsmanager create-secret --name jwt-secret --secret-string "your-secret"
```

### 2. Generate Secure Secret Key

```bash
# Linux/Mac - Generate 256-bit random key
openssl rand -base64 32

# PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))

# Online (use trusted source)
# https://randomkeygen.com/ (select 256-bit key)
```

**Minimum Requirements:**
- ? At least 32 characters
- ? Random, unpredictable
- ? Never committed to source control
- ? Different per environment (dev/staging/prod)

### 3. Update appsettings.Production.json

```json
{
  "JwtSettings": {
    "Secret": "*** REPLACE WITH ENV VARIABLE ***",
    "Issuer": "AuthService-Production",
    "Audience": "YourProductionApp",
    "ExpiryMinutes": 15
  },
  "ConnectionStrings": {
    "Default": "*** PRODUCTION DB CONNECTION ***"
  }
}
```

### 4. Test Checklist

#### Authentication
- [ ] User can login with valid credentials
- [ ] Login fails with invalid credentials
- [ ] Account locks after 5 failed attempts
- [ ] JWT token is returned on successful login

#### Authorization
- [ ] Admin user can access admin-only endpoints
- [ ] Regular user cannot access admin endpoints
- [ ] JWT contains role claims (inspect token at jwt.io)
- [ ] 401 returned for unauthenticated requests
- [ ] 403 returned for unauthorized requests

#### Token Refresh
- [ ] `/api/auth/refresh` returns new tokens with valid refresh token
- [ ] Old refresh token cannot be reused
- [ ] Expired refresh token returns 401
- [ ] New access token has fresh expiration time

#### Token Revocation
- [ ] Logout invalidates token immediately
- [ ] Logged-out token returns 401 on next API call
- [ ] Multiple sessions can be revoked at once

#### Security Headers
- [ ] `X-Frame-Options: DENY` present in response headers
- [ ] `X-Content-Type-Options: nosniff` present
- [ ] `Strict-Transport-Security` present (production/HTTPS only)

---

## Database Migration

**No schema changes required!** Refresh tokens use existing `Tokens` table.

Verify table exists:
```sql
SELECT * FROM Tokens WHERE TokenType = 'RefreshToken';
```

---

## Monitoring & Logging

### Key Metrics to Monitor
1. **Failed login attempts** - spike = brute force attack
2. **Token refresh rate** - unusually high = token leak
3. **401/403 responses** - pattern analysis
4. **Session count per user** - concurrent session tracking

### Critical Log Events
```csharp
// Already logged by the implementation:
- "Token validation failed: Session not found or revoked"
- "Invalid or expired refresh token attempt"
- "Token refreshed successfully for user {UserId}"
- "Unauthorized login attempt for user {UserName}"
```

---

## Rollback Plan

If issues occur after deployment:

1. **Authentication Failures**
   - Verify JWT secret is correctly set in environment
   - Check secret is at least 32 characters
   - Restart application to reload configuration

2. **Authorization Not Working**
   - Check user roles in database: `SELECT * FROM UserRoles`
   - Inspect JWT token at jwt.io - verify role claims present
   - Check authorization policies in Program.cs

3. **Refresh Token Issues**
   - Check `Tokens` table for RefreshToken entries
   - Verify refresh token expiration (30 days)
   - Check repository registration in DependencyInjection.cs

4. **Performance Issues**
   - JwtRevocationMiddleware queries DB on every request
   - Consider adding Redis cache for active sessions
   - Monitor database query performance

---

## Client-Side Implementation

### Recommended Token Storage

#### ? **DO NOT USE** localStorage
```javascript
// INSECURE - Vulnerable to XSS attacks
localStorage.setItem('token', accessToken); // ? DON'T DO THIS
```

#### ? **USE** httpOnly Cookies (Web Apps)
```csharp
// Server-side: Set cookie in AuthController
Response.Cookies.Append("access_token", token, new CookieOptions
{
    HttpOnly = true,
    Secure = true, // HTTPS only
    SameSite = SameSiteMode.Strict,
    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
});
```

```javascript
// Client-side: Cookies sent automatically
fetch('/api/protected', {
  credentials: 'include' // Send cookies
});
```

#### ? **USE** Secure Storage (Mobile Apps)
- iOS: Keychain
- Android: EncryptedSharedPreferences
- React Native: react-native-keychain

### Token Refresh Implementation

```javascript
let refreshTokenPromise = null;

async function fetchWithAuth(url, options = {}) {
  let response = await fetch(url, {
    ...options,
    headers: {
      ...options.headers,
      'Authorization': `Bearer ${getAccessToken()}`
    }
  });

  if (response.status === 401) {
    // Token expired, refresh it
    if (!refreshTokenPromise) {
      refreshTokenPromise = refreshAccessToken()
        .finally(() => { refreshTokenPromise = null; });
    }
    
    await refreshTokenPromise;
    
    // Retry original request with new token
    response = await fetch(url, {
      ...options,
      headers: {
        ...options.headers,
        'Authorization': `Bearer ${getAccessToken()}`
      }
    });
  }

  return response;
}

async function refreshAccessToken() {
  const response = await fetch('/api/auth/refresh', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken: getRefreshToken() })
  });

  if (response.ok) {
    const { accessToken, refreshToken } = await response.json();
    saveTokens(accessToken, refreshToken);
  } else {
    // Refresh failed, logout user
    clearTokens();
    window.location.href = '/login';
  }
}
```

---

## Security Incident Response

### If JWT Secret Key is Compromised

1. **Immediately generate new secret key**
   ```bash
   openssl rand -base64 32
   ```

2. **Update production environment variable**
   ```bash
   # Azure App Service
   az webapp config appsettings set --settings JWT_SECRET="new-secret"
   ```

3. **Restart application**
   - All existing tokens become invalid
   - Users must re-login

4. **Monitor for suspicious activity**
   - Check audit logs for unusual logins
   - Review active sessions

5. **Notify users** (if breach confirmed)
   - Force password reset
   - Enable MFA

---

## Performance Optimization

### Redis Cache for Sessions (Optional)

**Problem**: JwtRevocationMiddleware queries DB on every request

**Solution**: Cache active sessions in Redis

```csharp
// Add to Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
});

// Modified middleware
public async Task InvokeAsync(HttpContext context, IDistributedCache cache)
{
    var cacheKey = $"session:{token}";
    var cachedSession = await cache.GetStringAsync(cacheKey);
    
    if (cachedSession == null)
    {
        // Query DB and cache result
        var session = await sessionRepository.GetActiveSessionByTokenAsync(token);
        await cache.SetStringAsync(cacheKey, 
            JsonSerializer.Serialize(session),
            new DistributedCacheEntryOptions 
            { 
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) 
            });
    }
}
```

---

## Support Contacts

### Issue: Users Can't Login
**Check:**
1. JWT secret is set correctly
2. Database connection working
3. User account not locked/suspended
4. Check application logs for errors

### Issue: Authorization Always Fails
**Check:**
1. User has assigned roles in database
2. JWT token contains role claims (inspect at jwt.io)
3. Authorization policies configured correctly
4. Role names match exactly (case-sensitive)

### Issue: Tokens Not Refreshing
**Check:**
1. Refresh token in database (TokenType = 'RefreshToken')
2. Refresh token not expired (30 days)
3. RefreshTokenRepository registered in DI
4. Client sending correct refresh token

---

## ? Deployment Complete When:

- [ ] JWT secret set from environment variable
- [ ] Application builds successfully
- [ ] Login returns JWT with role claims
- [ ] Authorization policies work correctly
- [ ] Token refresh endpoint functional
- [ ] Logout immediately invalidates tokens
- [ ] Security headers present in responses
- [ ] HTTPS enabled (production)
- [ ] Monitoring/logging configured
- [ ] Backup/rollback plan tested

**Status: Ready for Production ?**
