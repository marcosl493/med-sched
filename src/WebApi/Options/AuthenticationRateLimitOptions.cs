namespace WebApi.Options
{
    public class AuthenticationRateLimitOptions
    {
        public readonly static string SectionName = "AuthenticationRateLimit";
        public int PermitLimit { get; set; }
        public int QueueLimit { get; set; }
        public int WindowSeconds { get; set; }
    }
}
