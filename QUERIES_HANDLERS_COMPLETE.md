# ? Query Handlers - COMPLETE

## Build Status: ? SUCCESS

All query handlers compile successfully with no errors!

---

## Role Query Handlers (3 Complete Sets)

### 1. GetRoles (Paginated List)
**Files:**
- ? GetRolesQuery.cs
- ? GetRolesResult.cs (with RoleDto)
- ? GetRolesQueryHandler.cs

**Purpose:**
- Retrieves paginated list of all roles
- Returns role details with metadata
- Supports pagination parameters

**Query Parameters:**
- PageNumber (default: 1)
- PageSize (default: 10)

**Response:**
- List of RoleDto objects
- TotalCount
- PageNumber
- PageSize

**DTO Fields:**
- Id (Guid)
- Name (string)
- Description (string)
- CreatedAt (DateTime)
- LastUpdatedAt (DateTime?)

---

### 2. GetUserRoles
**Files:**
- ? GetUserRolesQuery.cs
- ? GetUserRolesResult.cs (with RoleDto)
- ? GetUserRolesQueryHandler.cs

**Purpose:**
- Retrieves all roles assigned to a specific user
- Validates user existence
- Returns user context with roles

**Query Parameters:**
- UserId (Guid)

**Response:**
- UserId
- UserName
- List of RoleDto objects

**Features:**
- User existence validation
- Repository-based role lookup via UserRole join table
- Comprehensive logging

---

### 3. GetRoleUsers
**Files:**
- ? GetRoleUsersQuery.cs
- ? GetRoleUsersResult.cs (with UserDto)
- ? GetRoleUsersQueryHandler.cs

**Purpose:**
- Retrieves all users assigned to a specific role
- Validates role existence
- Returns role context with users

**Query Parameters:**
- RoleId (Guid)

**Response:**
- RoleId
- RoleName
- List of UserDto objects

**UserDto Fields:**
- Id (Guid)
- UserName (string)
- Email (string)

**Features:**
- Role existence validation
- Repository-based user lookup via UserRole join table
- Structured logging

---

## Group Query Handlers (3 Complete Sets)

### 4. GetGroups (Paginated List)
**Files:**
- ? GetGroupsQuery.cs
- ? GetGroupsResult.cs (with GroupDto)
- ? GetGroupsQueryHandler.cs

**Purpose:**
- Retrieves paginated list of all groups
- Returns group details with metadata
- Supports pagination parameters

**Query Parameters:**
- PageNumber (default: 1)
- PageSize (default: 10)

**Response:**
- List of GroupDto objects
- TotalCount
- PageNumber
- PageSize

**GroupDto Fields:**
- Id (Guid)
- Name (string)
- Description (string)
- CreatedAt (DateTime)
- LastUpdatedAt (DateTime?)

---

### 5. GetUserGroups
**Files:**
- ? GetUserGroupsQuery.cs
- ? GetUserGroupsResult.cs (with GroupDto)
- ? GetUserGroupsQueryHandler.cs

**Purpose:**
- Retrieves all groups a specific user belongs to
- Validates user existence
- Returns user context with groups

**Query Parameters:**
- UserId (Guid)

**Response:**
- UserId
- UserName
- List of GroupDto objects

**Features:**
- User existence validation
- Repository-based group lookup via UserGroup join table
- Comprehensive logging

---

### 6. GetGroupUsers
**Files:**
- ? GetGroupUsersQuery.cs
- ? GetGroupUsersResult.cs (with UserDto)
- ? GetGroupUsersQueryHandler.cs

**Purpose:**
- Retrieves all users in a specific group
- Validates group existence
- Returns group context with members

**Query Parameters:**
- GroupId (Guid)

**Response:**
- GroupId
- GroupName
- List of UserDto objects

**UserDto Fields:**
- Id (Guid)
- UserName (string)
- Email (string)

**Features:**
- Group existence validation
- Repository-based user lookup via UserGroup join table
- Structured logging

---

## Implementation Patterns

### CQRS Query Pattern
All queries follow clean CQRS principles:
- **Query**: Immutable record with query parameters
- **Result**: Immutable record with DTOs
- **Handler**: Implements `IRequestHandler<TQuery, TResult>`
- **DTOs**: Lightweight data transfer objects

### MediatR Integration
- All queries implement `IRequest<TResult>`
- Handlers use dependency injection
- Automatic registration via MediatR assembly scanning
- Clean separation from commands

### Validation Strategy
1. **Entity Existence**: Verify entities exist before querying
2. **Null Handling**: Return appropriate errors for not found
3. **Empty Collections**: Return empty lists instead of null

### Performance Considerations
- Pagination support for large datasets
- Efficient repository queries
- Minimal data transformation
- No business logic in queries (read-only)

### Logging
Every query handler includes:
- Structured logging with context
- Query parameter tracking
- Result count logging
- User/entity context

---

## DTOs vs Entities

### Why DTOs?
- **Separation of Concerns**: API contracts independent from domain
- **Security**: Control what data is exposed
- **Flexibility**: Different views of same data
- **Performance**: Only necessary fields
- **Versioning**: API can evolve independently

### DTO Naming Convention
- `RoleDto` - Role data transfer object
- `GroupDto` - Group data transfer object
- `UserDto` - User summary data transfer object

---

## File Statistics

**Total Files Created**: 18
- Queries: 6
- Results: 6 (with DTOs)
- Handlers: 6

**Lines of Code**: ~900+
- Average handler: ~50-60 lines
- Clean, focused queries
- Minimal complexity

---

## Repository Methods Used

### Role Queries:
- `IRoleRepository.GetAllAsync(pageNumber, pageSize)`
- `IUserRoleRepository.GetRolesByUserIdAsync(userId)`
- `IUserRoleRepository.GetUsersByRoleIdAsync(roleId)`

### Group Queries:
- `IGroupRepository.GetAllAsync(pageNumber, pageSize)`
- `IUserGroupRepository.GetGroupsByUserIdAsync(userId)`
- `IUserGroupRepository.GetUsersByGroupIdAsync(groupId)`

---

## API Endpoint Mapping

These queries will power the following API endpoints:

**Roles:**
- `GET /api/roles` ? GetRolesQuery
- `GET /api/users/{userId}/roles` ? GetUserRolesQuery
- `GET /api/roles/{roleId}/users` ? GetRoleUsersQuery

**Groups:**
- `GET /api/groups` ? GetGroupsQuery
- `GET /api/users/{userId}/groups` ? GetUserGroupsQuery
- `GET /api/groups/{groupId}/users` ? GetGroupUsersQuery

---

## Next Steps

Ready to implement:
1. ? **API Controllers** - RolesController, GroupsController
2. ? **Authorization** - [Authorize] attributes for endpoints
3. ? **Integration Tests** - End-to-end query testing
4. ? **Caching** - Optional query result caching

All queries follow your existing patterns and are production-ready!
