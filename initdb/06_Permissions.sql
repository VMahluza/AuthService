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
