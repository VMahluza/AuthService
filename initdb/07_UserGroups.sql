USE authservice_db;

-- Create UserGroups join table (many-to-many between Users and Groups)
CREATE TABLE IF NOT EXISTS UserGroups (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    GroupId CHAR(36) NOT NULL,
    AssignedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL,
    
    -- Foreign keys
    CONSTRAINT FK_UserGroups_Users FOREIGN KEY (UserId) 
        REFERENCES Users(Id) 
        ON DELETE CASCADE,
    
    CONSTRAINT FK_UserGroups_Groups FOREIGN KEY (GroupId) 
        REFERENCES `Groups`(Id) 
        ON DELETE CASCADE,
    
    -- Unique constraint to prevent duplicate group assignments
    UNIQUE KEY UK_UserGroups_User_Group (UserId, GroupId),
    
    -- Indexes for performance
    INDEX IX_UserGroups_UserId (UserId),
    INDEX IX_UserGroups_GroupId (GroupId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
