USE authservice_db;

CREATE TABLE IF NOT EXISTS AuditLogs (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    Action VARCHAR(150) NOT NULL,
    Description TEXT NULL,
    IpAddress VARCHAR(45) NULL,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);