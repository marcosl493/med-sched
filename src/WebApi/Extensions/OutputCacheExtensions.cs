namespace WebApi.Extensions;

public static class OutputCacheExtensions
{
    public static IServiceCollection AddOutputCacheConfiguration(this IServiceCollection services)
    {
        services.AddOutputCache(options =>
        {
            options.AddBasePolicy(builder =>
            {
                options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(30)));
            });
            options.AddPolicy(GetByIdPolicyName, builder =>
            {
                builder.Expire(TimeSpan.FromSeconds(30))
                       .SetVaryByRouteValue("id");
            });
        });
        return services;
    }
    public const string GetByIdPolicyName = "byId";
}
