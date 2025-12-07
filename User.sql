USE authservice_db;

CREATE TABLE IF NOT EXISTS Users (
    Id CHAR(36) PRIMARY KEY,
    UserName VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    FailedLoginAttempts INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL
);

