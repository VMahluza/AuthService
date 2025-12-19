USE authservice_db;

-- Create Groups table
CREATE TABLE IF NOT EXISTS `Groups` (
    Id CHAR(36) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT,
    CreatedAt DATETIME NOT NULL,
    LastUpdatedAt DATETIME NULL,
    
    -- Indexes
    INDEX IX_Groups_Name (Name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Insert default groups
INSERT IGNORE INTO `Groups` (Id, Name, Description, CreatedAt) VALUES
(UUID(), 'Administrators', 'System administrators group', UTC_TIMESTAMP()),
(UUID(), 'Standard Users', 'Default group for regular users', UTC_TIMESTAMP()),
(UUID(), 'Power Users', 'Users with elevated privileges', UTC_TIMESTAMP());
