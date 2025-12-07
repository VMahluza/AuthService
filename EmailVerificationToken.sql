USE authservice_db;

CREATE TABLE IF NOT EXISTS EmailVerificationTokens (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    Token VARCHAR(256) NOT NULL,  -- Base64 encoded 32 bytes is ~44 chars, but allowing some buffer
    ExpiresAt TIMESTAMP NOT NULL,
    UsedAt TIMESTAMP NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    LastUpdatedAt TIMESTAMP NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Optional: Index on Token for fast lookups during verification
CREATE INDEX IF NOT EXISTS IX_EmailVerificationTokens_Token ON EmailVerificationTokens(Token);

-- Optional: Index on UserId for querying tokens per user
CREATE INDEX IF NOT EXISTS IX_EmailVerificationTokens_UserId ON EmailVerificationTokens(UserId);