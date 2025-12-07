namespace AuthService.Domain.Settings;

public class JwtSettingsOptions
{

    public static readonly string SectionName = "JwtSettings";
    public required string Secret { get; set; }
    public required string Issuer { get; set; } 
    public required string Audience { get; set; } 
    public required int ExpiryMinutes { get; set; }
}