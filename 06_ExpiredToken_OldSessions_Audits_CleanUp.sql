USE authservice_db;

-- Event to automatically delete expired tokens older than 30 days
-- (MySQL Events must be enabled: SET GLOBAL event_scheduler = ON;)

DELIMITER $$

CREATE EVENT IF NOT EXISTS cleanup_expired_tokens
ON SCHEDULE EVERY 1 DAY
STARTS CURRENT_TIMESTAMP
DO
BEGIN
    DELETE FROM Tokens 
    WHERE ExpiresAt < DATE_SUB(NOW(), INTERVAL 30 DAY);
END$$

DELIMITER ;