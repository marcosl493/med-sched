using Application.UseCases.Login;

namespace WebApi.Extensions
{
    public static class MediatorExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<LoginResult>());
            return services;
        }
    }
}
