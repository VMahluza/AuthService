USE authservice_db;

-- Create Permissions table
CREATE TABLE IF NOT EXISTS Permissions (
    Id CHAR(36) PRIMARY KEY,
    `Key` VARCHAR(100) NOT NULL UNIQUE,
    Name VARCHAR(200) NOT NULL,
    Description TEXT,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL,
    
    -- Indexes
    INDEX IX_Permissions_Key (`Key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create GroupPermissions join table
CREATE TABLE IF NOT EXISTS GroupPermissions (
    Id CHAR(36) PRIMARY KEY,
    GroupId CHAR(36) NOT NULL,
    PermissionId CHAR(36) NOT NULL,
    AssignedByUserId CHAR(36) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL,
    
    -- Foreign keys
    CONSTRAINT FK_GroupPermissions_Groups FOREIGN KEY (GroupId) 
        REFERENCES `Groups`(Id) 
        ON DELETE CASCADE,
    
    CONSTRAINT FK_GroupPermissions_Permissions FOREIGN KEY (PermissionId) 
        REFERENCES Permissions(Id) 
        ON DELETE CASCADE,
    
    CONSTRAINT FK_GroupPermissions_Users FOREIGN KEY (AssignedByUserId) 
        REFERENCES Users(Id) 
        ON DELETE RESTRICT,
    
    -- Unique constraint to prevent duplicate assignments
    UNIQUE KEY UK_GroupPermissions_Group_Permission (GroupId, PermissionId),
    
    -- Indexes for performance
    INDEX IX_GroupPermissions_GroupId (GroupId),
    INDEX IX_GroupPermissions_PermissionId (PermissionId),
    INDEX IX_GroupPermissions_AssignedByUserId (AssignedByUserId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Insert some default permissions
INSERT IGNORE INTO Permissions (Id, `Key`, Name, Description, CreatedAt) VALUES
(UUID(), 'RESET_PASSWORD', 'Reset Password', 'Allows resetting user passwords', UTC_TIMESTAMP()),
(UUID(), 'DELETE_USER', 'Delete User', 'Allows deleting user accounts', UTC_TIMESTAMP()),
(UUID(), 'VIEW_AUDIT_LOGS', 'View Audit Logs', 'Allows viewing system audit logs', UTC_TIMESTAMP()),
(UUID(), 'MANAGE_ROLES', 'Manage Roles', 'Allows creating, updating, and deleting roles', UTC_TIMESTAMP()),
(UUID(), 'MANAGE_GROUPS', 'Manage Groups', 'Allows creating, updating, and deleting groups', UTC_TIMESTAMP()),
(UUID(), 'MANAGE_PERMISSIONS', 'Manage Permissions', 'Allows creating, updating, and assigning permissions', UTC_TIMESTAMP()),
(UUID(), 'VIEW_USERS', 'View Users', 'Allows viewing user information', UTC_TIMESTAMP()),
(UUID(), 'EDIT_USER', 'Edit User', 'Allows editing user information', UTC_TIMESTAMP());

-- =====================================================
-- CREATE ROLES AND GROUPS TABLES
-- =====================================================

-- Create Roles table
CREATE TABLE IF NOT EXISTS Roles (
    Id CHAR(36) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL,
    
    INDEX IX_Roles_Name (Name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create Groups table
CREATE TABLE IF NOT EXISTS `Groups` (
    Id CHAR(36) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL,
    
    INDEX IX_Groups_Name (Name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create UserRoles join table
CREATE TABLE IF NOT EXISTS UserRoles (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    RoleId CHAR(36) NOT NULL,
    AssignedAt DATETIME NOT NULL,
    AssignedBy CHAR(36) NULL,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL,
    
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) 
        REFERENCES Users(Id) 
        ON DELETE CASCADE,
    
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) 
        REFERENCES Roles(Id) 
        ON DELETE CASCADE,
    
    UNIQUE KEY UK_UserRoles_User_Role (UserId, RoleId),
    INDEX IX_UserRoles_UserId (UserId),
    INDEX IX_UserRoles_RoleId (RoleId),
    INDEX IX_UserRoles_AssignedBy (AssignedBy)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create UserGroups join table
CREATE TABLE IF NOT EXISTS UserGroups (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    GroupId CHAR(36) NOT NULL,
    JoinedAt DATETIME NOT NULL,
    AssignedBy CHAR(36) NULL,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL,
    
    CONSTRAINT FK_UserGroups_Users FOREIGN KEY (UserId) 
        REFERENCES Users(Id) 
        ON DELETE CASCADE,
    
    CONSTRAINT FK_UserGroups_Groups FOREIGN KEY (GroupId) 
        REFERENCES `Groups`(Id) 
        ON DELETE CASCADE,
    
    UNIQUE KEY UK_UserGroups_User_Group (UserId, GroupId),
    INDEX IX_UserGroups_UserId (UserId),
    INDEX IX_UserGroups_GroupId (GroupId),
    INDEX IX_UserGroups_AssignedBy (AssignedBy)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
