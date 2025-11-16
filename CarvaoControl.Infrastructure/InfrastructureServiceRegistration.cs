using Microsoft.Extensions.DependencyInjection;

namespace CarvaoControl.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        // Mantido apenas para compatibilidade; sem registro de SQLite.
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string? _ = null)
        {
            return services;
        }
    }
}