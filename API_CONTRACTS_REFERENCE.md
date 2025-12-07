# API Contracts - Quick Reference Guide

## Request/Response Patterns

### Role Management

#### Create Role
**Request:**
```json
POST /api/roles
{
  "name": "Manager",
  "description": "Department manager with elevated privileges"
}
```
**Response:** `RoleResponse`

#### Update Role
```json
PUT /api/roles/{roleId}
{
  "name": "Senior Manager",
  "description": "Updated description"
}
```
**Response:** `RoleResponse`

#### Delete Role
```
DELETE /api/roles/{roleId}
```
**Response:** `DeleteResponse`

#### Assign Role to User
```json
POST /api/roles/assign
{
  "userId": "guid-here",
  "roleId": "guid-here"
}
```
**Response:** `AssignRoleResponse`

#### Remove Role from User
```
DELETE /api/roles/{roleId}/users/{userId}
```
**Response:** `DeleteResponse`

#### Get All Roles (Paginated)
```
GET /api/roles?pageNumber=1&pageSize=10
```
**Response:** `RolesListResponse`

#### Get User's Roles
```
GET /api/users/{userId}/roles
```
**Response:** `UserRoleResponse`

#### Get Users with Specific Role
```
GET /api/roles/{roleId}/users
```
**Response:** `RoleUsersResponse`

---

### Group Management

#### Create Group
```json
POST /api/groups
{
  "name": "Finance Department",
  "description": "Financial operations team"
}
```
**Response:** `GroupResponse`

#### Update Group
```json
PUT /api/groups/{groupId}
{
  "name": "Finance & Accounting",
  "description": "Updated description"
}
```
**Response:** `GroupResponse`

#### Delete Group
```
DELETE /api/groups/{groupId}
```
**Response:** `DeleteResponse`

#### Add User to Group
```json
POST /api/groups/members
{
  "userId": "guid-here",
  "groupId": "guid-here"
}
```
**Response:** `AddUserToGroupResponse`

#### Remove User from Group
```
DELETE /api/groups/{groupId}/users/{userId}
```
**Response:** `DeleteResponse`

#### Get All Groups (Paginated)
```
GET /api/groups?pageNumber=1&pageSize=10
```
**Response:** `GroupsListResponse`

#### Get User's Groups
```
GET /api/users/{userId}/groups
```
**Response:** `UserGroupResponse`

#### Get Users in Specific Group
```
GET /api/groups/{groupId}/users
```
**Response:** `GroupUsersResponse`

---

## Response Examples

### RoleResponse
```json
{
  "id": "guid-here",
  "name": "Manager",
  "description": "Department manager",
  "createdAt": "2024-01-15T10:30:00Z",
  "lastUpdatedAt": "2024-01-20T14:15:00Z"
}
```

### RolesListResponse
```json
{
  "roles": [
    {
      "id": "guid-1",
      "name": "Admin",
      "description": "Administrator",
      "createdAt": "2024-01-10T09:00:00Z",
      "lastUpdatedAt": null
    },
    {
      "id": "guid-2",
      "name": "Manager",
      "description": "Department manager",
      "createdAt": "2024-01-15T10:30:00Z",
      "lastUpdatedAt": "2024-01-20T14:15:00Z"
    }
  ],
  "totalCount": 15,
  "pageNumber": 1,
  "pageSize": 10
}
```

### UserRoleResponse
```json
{
  "userId": "guid-here",
  "userName": "john.doe",
  "roles": [
    {
      "id": "role-guid-1",
      "name": "Manager",
      "description": "Department manager",
      "createdAt": "2024-01-15T10:30:00Z",
      "lastUpdatedAt": null
    },
    {
      "id": "role-guid-2",
      "name": "ProjectLead",
      "description": "Project leader",
      "createdAt": "2024-01-20T11:00:00Z",
      "lastUpdatedAt": null
    }
  ]
}
```

### GroupResponse
```json
{
  "id": "guid-here",
  "name": "Finance Department",
  "description": "Financial operations team",
  "createdAt": "2024-01-10T08:00:00Z",
  "lastUpdatedAt": "2024-01-25T16:30:00Z"
}
```

### AssignRoleResponse
```json
{
  "success": true,
  "message": "Role 'Manager' successfully assigned to user",
  "userId": "user-guid",
  "roleId": "role-guid",
  "roleName": "Manager"
}
```

### DeleteResponse
```json
{
  "success": true,
  "message": "Role deleted successfully"
}
```

### RoleUsersResponse
```json
{
  "roleId": "role-guid",
  "roleName": "Manager",
  "users": [
    {
      "id": "user-guid-1",
      "userName": "john.doe",
      "email": "john.doe@example.com"
    },
    {
      "id": "user-guid-2",
      "userName": "jane.smith",
      "email": "jane.smith@example.com"
    }
  ]
}
```

---

## Validation Rules

### Name Fields
- **Required**
- **MinLength**: 2 characters
- **MaxLength**: 100 characters

### Description Fields
- **Optional**
- **MaxLength**: 500 characters

### Guid Fields (UserId, RoleId, GroupId)
- **Required**
- Must be valid GUID format

---

## Error Responses

All endpoints should return appropriate HTTP status codes:

- **200 OK**: Successful GET/PUT
- **201 Created**: Successful POST
- **204 No Content**: Successful DELETE
- **400 Bad Request**: Validation errors
- **401 Unauthorized**: Missing/invalid authentication
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **409 Conflict**: Duplicate name or already assigned
- **500 Internal Server Error**: Server error

Example error response:
```json
{
  "success": false,
  "message": "Role with name 'Manager' already exists."
}
```
