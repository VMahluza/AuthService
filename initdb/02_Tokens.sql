USE authservice_db;

-- Drop the old EmailVerificationTokens table if it exists
DROP TABLE IF EXISTS EmailVerificationTokens;

-- Create unified Tokens table with Single Table Inheritance (STI) pattern
CREATE TABLE IF NOT EXISTS Tokens (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    Token VARCHAR(256) NOT NULL,
    TokenType VARCHAR(50) NOT NULL,  -- 'EmailVerification' or 'PasswordReset'
    ExpiresAt DATETIME NOT NULL,
    UsedAt DATETIME NULL,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL,
    
    -- Foreign key with cascade delete
    CONSTRAINT FK_Tokens_Users FOREIGN KEY (UserId) 
        REFERENCES Users(Id) 
        ON DELETE CASCADE,
    
    -- Indexes for performance
    INDEX IX_Tokens_Token (Token),
    INDEX IX_Tokens_UserId_TokenType (UserId, TokenType),
    INDEX IX_Tokens_TokenType_ExpiresAt (TokenType, ExpiresAt),
    INDEX IX_Tokens_UserId_UsedAt (UserId, UsedAt),
    
    -- Unique constraint to prevent duplicate active tokens
    UNIQUE KEY UK_Tokens_Token_TokenType (Token, TokenType)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Add check constraint for TokenType enum values (MySQL 8.0.16+)
ALTER TABLE Tokens 
    ADD CONSTRAINT CHK_Tokens_TokenType 
    CHECK (TokenType IN ('EmailVerification', 'PasswordReset'));