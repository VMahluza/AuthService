# HTTP Test Files

This folder contains organized HTTP test files for testing the AuthService API endpoints using Visual Studio's built-in HTTP client or REST Client extensions.

## Folder Structure

```
Http/
??? Auth/                    # Authentication endpoints
?   ??? Register/
?   ??? Login/
?   ??? Logout/
?   ??? RefreshToken/
?   ??? Password/
?   ??? Verify/
?   ??? AuditLogs/
??? Roles/                   # Role management endpoints
?   ??? CreateRole/
?   ??? UpdateRole/
?   ??? GetRole/
?   ??? AssignRole/
?   ??? DeleteRole/
??? Groups/                  # Group management endpoints
    ??? CreateGroup/
    ??? UpdateGroup/
    ??? GetGroup/
    ??? AddUserToGroup/
    ??? DeleteGroup/
```

## How to Use

### Prerequisites
1. Ensure the API is running on `http://localhost:5102`
2. Update the `@host` variable if using a different URL

### Getting Started

#### 1. Register a New User
```http
# File: Http/Auth/Register/auth.register.http
POST {{host}}/api/auth/register
Content-Type: application/json

{
    "userName": "testuser",
    "email": "test@example.com",
    "password": "SecurePassword123!",
    "confirmPassword": "SecurePassword123!"
}
```

#### 2. Verify Email
Check your email for the verification link and token, then:
```http
# File: Http/Auth/Verify/auth.verify-email.http
GET {{host}}/api/auth/verify-email?token=YOUR_VERIFICATION_TOKEN_HERE
```

#### 3. Login
```http
# File: Http/Auth/Login/auth.login.http
POST {{host}}/api/auth/login
Content-Type: application/json

{
    "userName": "testuser",
    "password": "SecurePassword123!"
}
```

**Important:** Copy the `accessToken` from the login response and use it for authenticated requests.

#### 4. Using the JWT Token

Replace `YOUR_JWT_TOKEN_HERE` in the files with your actual token:

```http
@token = eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

GET {{host}}/api/roles
Authorization: Bearer {{token}}
```

## Variables

Each HTTP file uses variables for reusability:

- `@host` - API base URL (default: `http://localhost:5102`)
- `@token` - Your JWT access token
- `@userId` - User GUID
- `@roleId` - Role GUID
- `@groupId` - Group GUID

## Endpoint Categories

### Authentication Endpoints (`/api/auth`)

| File | Method | Endpoint | Auth Required |
|------|--------|----------|--------------|
| `auth.register.http` | POST | `/api/auth/register` | No |
| `auth.verify-email.http` | GET | `/api/auth/verify-email` | No |
| `auth.login.http` | POST | `/api/auth/login` | No |
| `auth.logout.http` | DELETE | `/api/auth/logout` | No |
| `auth.refresh-token.http` | POST | `/api/auth/refresh` | No |
| `auth.forgot-password.http` | POST | `/api/auth/forgot-password` | No |
| `auth.reset-password.http` | POST | `/api/auth/reset-password` | No |
| `auth.audit-logs.http` | GET | `/api/auth/audit-logs/{userId}` | Yes |

### Role Management Endpoints (`/api/roles`)

| File | Method | Endpoint | Auth Required | Admin Only |
|------|--------|----------|--------------|------------|
| `role.get-all.http` | GET | `/api/roles` | Yes | No |
| `role.get-users.http` | GET | `/api/roles/{roleId}/users` | Yes | No |
| `role.get-user-roles.http` | GET | `/api/roles/users/{userId}` | Yes | No |
| `role.create.http` | POST | `/api/roles` | Yes | Yes |
| `role.update.http` | PUT | `/api/roles/{roleId}` | Yes | Yes |
| `role.assign.http` | POST | `/api/roles/assign` | Yes | Yes |
| `role.remove-user.http` | DELETE | `/api/roles/{roleId}/users/{userId}` | Yes | Yes |
| `role.delete.http` | DELETE | `/api/roles/{roleId}` | Yes | Yes |

### Group Management Endpoints (`/api/groups`)

| File | Method | Endpoint | Auth Required | Admin Only |
|------|--------|----------|--------------|------------|
| `group.get-all.http` | GET | `/api/groups` | Yes | No |
| `group.get-users.http` | GET | `/api/groups/{groupId}/users` | Yes | No |
| `group.get-user-groups.http` | GET | `/api/groups/users/{userId}` | Yes | No |
| `group.create.http` | POST | `/api/groups` | Yes | Yes |
| `group.update.http` | PUT | `/api/groups/{groupId}` | Yes | Yes |
| `group.add-user.http` | POST | `/api/groups/members` | Yes | Yes |
| `group.remove-user.http` | DELETE | `/api/groups/{groupId}/users/{userId}` | Yes | Yes |
| `group.delete.http` | DELETE | `/api/groups/{groupId}` | Yes | Yes |

## Testing Workflow

### Complete Authentication Flow

1. **Register** ? `auth.register.http`
2. **Verify Email** ? `auth.verify-email.http`
3. **Login** ? `auth.login.http` (save the token)
4. **Use Protected Endpoints** ? Use token in Authorization header
5. **Refresh Token** ? `auth.refresh-token.http` (when token expires)
6. **Logout** ? `auth.logout.http`

### Role Management Flow

1. **Login as Admin** ? Get admin token
2. **Create Role** ? `role.create.http`
3. **Assign Role to User** ? `role.assign.http`
4. **View Role Users** ? `role.get-users.http`
5. **Update Role** ? `role.update.http`
6. **Remove Role from User** ? `role.remove-user.http`
7. **Delete Role** ? `role.delete.http`

### Group Management Flow

1. **Login as Admin** ? Get admin token
2. **Create Group** ? `group.create.http`
3. **Add User to Group** ? `group.add-user.http`
4. **View Group Members** ? `group.get-users.http`
5. **Update Group** ? `group.update.http`
6. **Remove User from Group** ? `group.remove-user.http`
7. **Delete Group** ? `group.delete.http`

## Tips

1. **Replace Variables**: Before running requests, replace placeholder values:
   - `YOUR_JWT_TOKEN_HERE` with actual token
   - `YOUR_USER_ID_HERE` with actual GUID
   - `YOUR_ROLE_ID_HERE` with actual GUID
   - `YOUR_GROUP_ID_HERE` with actual GUID

2. **Use Separators**: Use `###` to separate multiple requests in the same file

3. **Admin Access**: Many endpoints require Admin role. Create an admin user in your database or through your seeding logic.

4. **Token Expiration**: Tokens expire based on `JwtSettings:ExpiryMinutes`. Use the refresh token endpoint to get a new token.

5. **Visual Studio**: Use the "Send Request" option that appears above each HTTP request when you open these files in Visual Studio.

## Environment-Specific Configuration

For different environments, update the `@host` variable:

```http
# Development
@host = http://localhost:5102

# Staging
@host = https://staging-api.yourcompany.com

# Production
@host = https://api.yourcompany.com
```

## Troubleshooting

### 401 Unauthorized
- Ensure you're using a valid, non-expired token
- Check that the token is properly formatted: `Bearer {token}`
- Verify the user has the required role/permissions

### 403 Forbidden
- The endpoint requires Admin role
- Ensure your user has been assigned the Admin role

### 400 Bad Request
- Check request body format matches the expected schema
- Verify all required fields are present
- Ensure GUIDs are in valid format

### 404 Not Found
- Verify the resource ID (userId, roleId, groupId) exists
- Check the endpoint URL is correct

## Security Notes

?? **Important:**
- Never commit files with actual tokens or passwords to version control
- Use environment variables or secure vaults for sensitive data
- Rotate tokens regularly
- Use HTTPS in production environments
