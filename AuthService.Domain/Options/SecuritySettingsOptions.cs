
namespace AuthService.Domain.Options;
public class SecuritySettingsOptions
{

        public static readonly string SectionName = "SecuritySettings";
        public required int MaxFailedAccessAttempts { get; set; }
        public required int DefaultLockoutTimeSpanInMinutes { get; set; }
        public int MaxConcurrentSessions { get; set; } = 0;

    public SessionEnforcementStrategy SessionEnforcement { get; set; } = SessionEnforcementStrategy.RevokeOldest;
}

