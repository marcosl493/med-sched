namespace Infrastructure.Authentication;

public class JwtOptions
{
    public static readonly string SectionName = "Jwt";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int ExpiresInMinutes { get; set; }
}
