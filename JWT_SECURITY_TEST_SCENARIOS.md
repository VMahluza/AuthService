# JWT Security - Test Scenarios & Expected Behaviors

## Test Scenario 1: Login with Role Claims

### Setup
```sql
-- Create test users with different roles
INSERT INTO Users (Id, UserName, Email, PasswordHash, Status, FailedLoginAttempts, CreatedAt, LastUpdatedAt)
VALUES 
  ('11111111-1111-1111-1111-111111111111', 'admin', 'admin@test.com', '[hashed]', 'Active', 0, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
  ('22222222-2222-2222-2222-222222222222', 'manager', 'manager@test.com', '[hashed]', 'Active', 0, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
  ('33333333-3333-3333-3333-333333333333', 'user', 'user@test.com', '[hashed]', 'Active', 0, UTC_TIMESTAMP(), UTC_TIMESTAMP());

-- Assign roles
INSERT INTO UserRoles (UserId, RoleId, AssignedAt)
VALUES 
  ('11111111-1111-1111-1111-111111111111', '[admin-role-id]', UTC_TIMESTAMP()),
  ('22222222-2222-2222-2222-222222222222', '[manager-role-id]', UTC_TIMESTAMP()),
  ('33333333-3333-3333-3333-333333333333', '[user-role-id]', UTC_TIMESTAMP());
```

### Test Steps

#### Step 1: Admin Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "userName": "admin",
  "password": "Test123!"
}
```

**Expected Response:**
```json
{
  "userId": "11111111-1111-1111-1111-111111111111",
  "userName": "admin",
  "email": "admin@test.com",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### Step 2: Decode JWT Token
Go to https://jwt.io and paste the `accessToken`

**Expected Claims:**
```json
{
  "sub": "11111111-1111-1111-1111-111111111111",
  "unique_name": "admin",
  "email": "admin@test.com",
  "jti": "some-guid",
  "role": "Admin",  // ? This is the critical part!
  "exp": 1234567890,
  "iss": "AuthService",
  "aud": "AuthServiceClients"
}
```

? **PASS**: Role claim present
? **FAIL**: Role claim missing ? Authorization will not work

---

## Test Scenario 2: Authorization with Roles

### Test 1: Admin Access Admin Endpoint

```http
GET /api/roles
Authorization: Bearer [admin-jwt-token]
```

**Expected:**
- Status: `200 OK`
- Response: List of roles

### Test 2: Manager Access Admin Endpoint

```http
GET /api/roles
Authorization: Bearer [manager-jwt-token]
```

**Expected:**
- Status: `403 Forbidden`
- Response: Authorization error

### Test 3: User Access Manager Endpoint

```http
GET /api/groups
Authorization: Bearer [user-jwt-token]
```

**Expected:**
- Status: `403 Forbidden`

### Test 4: No Token

```http
GET /api/roles
```

**Expected:**
- Status: `401 Unauthorized`

---

## Test Scenario 3: Token Refresh Flow

### Step 1: Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "userName": "admin",
  "password": "Test123!"
}
```

**Save:**
- `accessToken` (short-lived, 15 minutes)
- Note: Refresh token is stored in DB, not returned in response

### Step 2: Query Refresh Token from DB
```sql
SELECT Token FROM Tokens 
WHERE UserId = '11111111-1111-1111-1111-111111111111' 
  AND TokenType = 'RefreshToken' 
  AND UsedAt IS NULL
ORDER BY CreatedAt DESC 
LIMIT 1;
```

**Save:** `refreshToken` value

### Step 3: Wait for Access Token to Expire
```bash
# Or manually wait 15+ minutes
# For testing, you can reduce ExpiryMinutes in appsettings.json to 1 minute
```

### Step 4: Try to Access Protected Endpoint
```http
GET /api/roles
Authorization: Bearer [expired-access-token]
```

**Expected:**
- Status: `401 Unauthorized`
- Message: "Token has been revoked or session expired"

### Step 5: Refresh Token
```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "[refresh-token-from-db]"
}
```

**Expected Response:**
```json
{
  "accessToken": "new-jwt-token...",
  "refreshToken": "new-refresh-token...",
  "expiresAt": "2024-01-15T12:15:00Z"
}
```

### Step 6: Verify Old Refresh Token is Consumed
```sql
SELECT * FROM Tokens 
WHERE Token = '[old-refresh-token]';
```

**Expected:**
- `UsedAt` column should have a timestamp
- Token cannot be reused

### Step 7: Use New Access Token
```http
GET /api/roles
Authorization: Bearer [new-access-token]
```

**Expected:**
- Status: `200 OK`
- Roles returned successfully

### Step 8: Try to Reuse Old Refresh Token
```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "[old-refresh-token]"
}
```

**Expected:**
- Status: `401 Unauthorized`
- Message: "Invalid or expired refresh token"

---

## Test Scenario 4: Token Revocation (Logout)

### Step 1: Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "userName": "admin",
  "password": "Test123!"
}
```

**Save:** `accessToken`

### Step 2: Verify Token Works
```http
GET /api/roles
Authorization: Bearer [access-token]
```

**Expected:** `200 OK`

### Step 3: Logout
```http
DELETE /api/auth/logout
Content-Type: application/json

{
  "jwtToken": "[access-token]",
  "revokeAllSessions": false
}
```

**Expected:**
```json
{
  "isSuccessful": true,
  "message": "Logout Successful!"
}
```

### Step 4: Try to Use Token Again
```http
GET /api/roles
Authorization: Bearer [access-token]
```

**Expected:**
- Status: `401 Unauthorized`
- Message: "Token has been revoked or session expired. Please login again."

? **PASS**: Token immediately invalid after logout
? **FAIL**: Token still works ? Revocation middleware not active

### Step 5: Verify Session in Database
```sql
SELECT * FROM UserSessions 
WHERE JwtToken = '[access-token]';
```

**Expected:**
- Session deleted OR
- `RevokedAt` timestamp present

---

## Test Scenario 5: Concurrent Session Management

### Setup: Configure Max Sessions
```json
// appsettings.json
{
  "SecuritySettings": {
    "MaxConcurrentSessions": 2,
    "SessionEnforcement": "RevokeOldest"
  }
}
```

### Test Steps

#### Session 1: Login from Device 1
```http
POST /api/auth/login

{ "userName": "admin", "password": "Test123!" }
```

**Save:** `token1`

#### Session 2: Login from Device 2
```http
POST /api/auth/login

{ "userName": "admin", "password": "Test123!" }
```

**Save:** `token2`

#### Session 3: Login from Device 3 (Exceeds Limit)
```http
POST /api/auth/login

{ "userName": "admin", "password": "Test123!" }
```

**Save:** `token3`

#### Verify Session 1 is Revoked
```http
GET /api/roles
Authorization: Bearer [token1]
```

**Expected:**
- Status: `401 Unauthorized`
- Reason: Oldest session revoked

#### Verify Sessions 2 & 3 Still Work
```http
GET /api/roles
Authorization: Bearer [token2]
```

**Expected:** `200 OK`

```http
GET /api/roles
Authorization: Bearer [token3]
```

**Expected:** `200 OK`

---

## Test Scenario 6: Security Headers

### Test
```http
GET /api/auth/login
```

### Verify Response Headers
```http
HTTP/1.1 200 OK
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Referrer-Policy: strict-origin-when-cross-origin
Strict-Transport-Security: max-age=31536000; includeSubDomains
```

? **PASS**: All security headers present
?? **WARNING**: HSTS should only be on HTTPS/production

---

## Test Scenario 7: Weak JWT Secret Detection

### Test: Start Application with Short Secret

```json
// appsettings.json
{
  "JwtSettings": {
    "Secret": "short"  // Only 5 characters
  }
}
```

### Expected Result
Application should **FAIL TO START** with error:
```
System.InvalidOperationException: JWT Secret key must be at least 32 characters long.
```

? **PASS**: Application refuses to start
? **FAIL**: Application starts ? Weak key vulnerability

---

## Test Scenario 8: Role Assignment Changes

### Step 1: Login as Regular User
```http
POST /api/auth/login

{ "userName": "user", "password": "Test123!" }
```

**JWT Claims:**
```json
{
  "role": "User"
}
```

### Step 2: Admin Promotes User to Admin
```http
POST /api/roles/assign
Authorization: Bearer [admin-token]

{
  "userId": "33333333-3333-3333-3333-333333333333",
  "roleId": "[admin-role-id]"
}
```

### Step 3: User Tries to Access Admin Endpoint with OLD Token
```http
GET /api/roles
Authorization: Bearer [user-old-token]
```

**Expected:**
- Status: `403 Forbidden`
- Reason: Old JWT still has "User" role

### Step 4: User Logs Out and Logs In Again
```http
DELETE /api/auth/logout

POST /api/auth/login
{ "userName": "user", "password": "Test123!" }
```

### Step 5: Decode NEW JWT
**Expected Claims:**
```json
{
  "role": ["User", "Admin"]  // ? Both roles now
}
```

### Step 6: User Accesses Admin Endpoint with NEW Token
```http
GET /api/roles
Authorization: Bearer [user-new-token]
```

**Expected:**
- Status: `200 OK`

**Key Insight:** Role changes require new login to take effect (by design, for security)

---

## Test Scenario 9: Account Lockout

### Step 1: Attempt Login with Wrong Password 5 Times
```http
POST /api/auth/login

{ "userName": "admin", "password": "WrongPassword" }
```

Repeat 5 times.

### Step 2: Verify Account Locked
```sql
SELECT Status, FailedLoginAttempts FROM Users 
WHERE UserName = 'admin';
```

**Expected:**
- `Status`: "Locked"
- `FailedLoginAttempts`: 5

### Step 3: Try to Login with CORRECT Password
```http
POST /api/auth/login

{ "userName": "admin", "password": "Test123!" }
```

**Expected:**
- Status: `401 Unauthorized`
- Message: "The Account is locked. please contact support"

? **PASS**: Account locked, even with correct password
? **FAIL**: Login succeeds ? Lockout not working

---

## Test Scenario 10: Email Verification Required

### Step 1: Register New User
```http
POST /api/auth/register

{
  "userName": "newuser",
  "email": "new@test.com",
  "password": "Test123!"
}
```

### Step 2: Try to Login Before Email Verification
```http
POST /api/auth/login

{ "userName": "newuser", "password": "Test123!" }
```

**Expected:**
- Status: `401 Unauthorized`
- Message: "The Account is pending verification. please verify your account"

### Step 3: Verify Email
```http
GET /api/auth/verify-email?token=[verification-token]
```

### Step 4: Login After Verification
```http
POST /api/auth/login

{ "userName": "newuser", "password": "Test123!" }
```

**Expected:**
- Status: `200 OK`
- Access token returned

---

## Performance Test Scenarios

### Test 1: Middleware Performance
**Goal:** Ensure JwtRevocationMiddleware doesn't cause slowdown

```bash
# Load test tool (Apache Bench, k6, JMeter)
ab -n 1000 -c 10 -H "Authorization: Bearer [token]" http://localhost:5000/api/roles
```

**Benchmark:**
- Without middleware: ~500 req/sec
- With middleware: Should be ~450+ req/sec

**If significantly slower:** Consider Redis caching

### Test 2: Refresh Token Under Load
```bash
# Simulate 100 users refreshing tokens simultaneously
ab -n 100 -c 10 -p refresh.json http://localhost:5000/api/auth/refresh
```

**Expected:**
- All requests succeed
- No duplicate tokens issued
- Old refresh tokens marked as used

---

## Security Vulnerability Tests

### Test 1: XSS via JWT Claims
**Attempt:** Inject script into username
```http
POST /api/auth/register

{
  "userName": "<script>alert('xss')</script>",
  "email": "test@test.com",
  "password": "Test123!"
}
```

**Expected:** Username sanitized or rejected

### Test 2: SQL Injection via Login
```http
POST /api/auth/login

{
  "userName": "admin' OR '1'='1",
  "password": "anything"
}
```

**Expected:**
- Login fails (using parameterized queries)
- No unauthorized access

### Test 3: Timing Attack on Login
**Attempt:** Measure response time to enumerate users

```bash
time curl -X POST /api/auth/login -d '{"userName":"exists","password":"wrong"}'
time curl -X POST /api/auth/login -d '{"userName":"notexists","password":"wrong"}'
```

**Expected:** Similar response times (constant-time comparison)

---

## ? All Tests Passing Checklist

- [ ] Login returns JWT with role claims
- [ ] Admin can access admin-only endpoints
- [ ] Non-admin cannot access admin endpoints
- [ ] Token refresh works and rotates tokens
- [ ] Old refresh tokens cannot be reused
- [ ] Logout immediately invalidates tokens
- [ ] Revoked tokens return 401
- [ ] Security headers present in all responses
- [ ] Weak JWT secret rejected at startup
- [ ] Account locks after 5 failed attempts
- [ ] Unverified accounts cannot login
- [ ] Concurrent session limit enforced

**Status:** 
- ? ALL PASS ? Production Ready
- ? ANY FAIL ? Review implementation
