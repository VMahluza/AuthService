USE authservice_db;

CREATE TABLE IF NOT EXISTS EmailVerificationTokens (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    Token VARCHAR(256) NOT NULL,  -- Base64 encoded 32 bytes is ~44 chars, but allowing some buffer
    ExpiresAt DATETIME NOT NULL,
    UsedAt DATETIME NULL,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
