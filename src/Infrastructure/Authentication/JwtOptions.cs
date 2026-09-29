using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Authentication;

public class JwtOptions
{
    public static readonly string SectionName = "Jwt";
    [Required]
    public string Issuer { get; set; } = string.Empty;
    [Required]
    public string Audience { get; set; } = string.Empty;
    [Required]  
    public string SecretKey { get; set; } = string.Empty;
    [Required]
    public int ExpiresInMinutes { get; set; }
}
