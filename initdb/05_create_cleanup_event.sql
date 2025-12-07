USE authservice_db;

-- Enable the event scheduler (needs to be run once)
SET GLOBAL event_scheduler = ON;

-- Create a scheduled event to delete expired audit logs
DROP EVENT IF EXISTS cleanup_expired_audit_logs;

CREATE EVENT cleanup_expired_audit_logs
ON SCHEDULE EVERY 1 DAY
STARTS (TIMESTAMP(CURRENT_DATE) + INTERVAL 1 DAY + INTERVAL 2 HOUR) -- Runs at 2 AM daily
DO
BEGIN
    DELETE FROM AuditLogs WHERE ExpiresAt < NOW();
END;