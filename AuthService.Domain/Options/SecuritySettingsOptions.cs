
namespace AuthService.Domain.Options;
public class SecuritySettingsOptions
{

        public static readonly string SectionName = "SecuritySettings";
        public required int MaxFailedAccessAttempts { get; set; }
        public required int DefaultLockoutTimeSpanInMinutes { get; set; }
    }

