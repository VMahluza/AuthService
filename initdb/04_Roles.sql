USE authservice_db;

-- Create Roles table
CREATE TABLE IF NOT EXISTS Roles (
    Id CHAR(36) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL,
    
    -- Indexes
    INDEX IX_Roles_Name (Name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Insert default roles
INSERT IGNORE INTO Roles (Id, Name, Description, CreatedAt) VALUES
(UUID(), 'Admin', 'Administrator with full system access', UTC_TIMESTAMP()),
(UUID(), 'User', 'Standard user with basic access', UTC_TIMESTAMP()),
(UUID(), 'Manager', 'Manager with elevated permissions', UTC_TIMESTAMP());
