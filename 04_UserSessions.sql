USE authservice_db;

CREATE TABLE IF NOT EXISTS UserSessions (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    JwtToken TEXT NULL,
    IssuedAt DATETIME NOT NULL,
    ExpiresAt DATETIME NOT NULL,
    RevokedAt DATETIME NULL,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL,
    
    -- Foreign key with CASCADE DELETE
    CONSTRAINT FK_UserSessions_Users 
        FOREIGN KEY (UserId) 
        REFERENCES Users(Id) 
        ON DELETE CASCADE,
    
    -- Index on UserId for faster lookups by user
    INDEX IX_UserSessions_UserId (UserId),
    
    -- Index on ExpiresAt for cleanup queries
    INDEX IX_UserSessions_ExpiresAt (ExpiresAt),
    
    -- Index on RevokedAt for filtering active sessions
    INDEX IX_UserSessions_RevokedAt (RevokedAt),
    
    -- Composite index for finding active sessions by user
    INDEX IX_UserSessions_UserId_RevokedAt_ExpiresAt (UserId, RevokedAt, ExpiresAt)
);