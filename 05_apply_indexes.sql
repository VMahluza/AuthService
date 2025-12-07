USE authservice_db;

-- Indexes for Users table
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_UserName ON Users(UserName);
CREATE INDEX IX_Users_Status ON Users(Status);
CREATE INDEX IX_Users_CreatedAt ON Users(CreatedAt);

-- Indexes for EmailVerificationTokens table
-- (Token and UserId indexes already created in 02_EmailVerificationTokens.sql)
CREATE INDEX IX_EmailVerificationTokens_ExpiresAt ON EmailVerificationTokens(ExpiresAt);
CREATE INDEX IX_EmailVerificationTokens_UsedAt ON EmailVerificationTokens(UsedAt);

-- Indexes for AuditLogs table
CREATE INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
CREATE INDEX IX_AuditLogs_Action ON AuditLogs(Action);
CREATE INDEX IX_AuditLogs_CreatedAt ON AuditLogs(CreatedAt);
CREATE INDEX IX_AuditLogs_IpAddress ON AuditLogs(IpAddress);
CREATE INDEX IX_AuditLogs_ExpiresAt ON AuditLogs(ExpiresAt); -- Important for cleanup performance

-- Composite indexes for common query patterns
CREATE INDEX IX_AuditLogs_UserId_CreatedAt ON AuditLogs(UserId, CreatedAt);
CREATE INDEX IX_EmailVerificationTokens_UserId_ExpiresAt ON EmailVerificationTokens(UserId, ExpiresAt);