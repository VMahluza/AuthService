USE authservice_db;

-- Create UserRoles join table (many-to-many between Users and Roles)
CREATE TABLE IF NOT EXISTS UserRoles (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    RoleId CHAR(36) NOT NULL,
    AssignedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL,
    
    -- Foreign keys
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) 
        REFERENCES Users(Id) 
        ON DELETE CASCADE,
    
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) 
        REFERENCES Roles(Id) 
        ON DELETE CASCADE,
    
    -- Unique constraint to prevent duplicate role assignments
    UNIQUE KEY UK_UserRoles_User_Role (UserId, RoleId),
    
    -- Indexes for performance
    INDEX IX_UserRoles_UserId (UserId),
    INDEX IX_UserRoles_RoleId (RoleId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
