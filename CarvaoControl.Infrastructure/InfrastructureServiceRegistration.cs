using Microsoft.Extensions.DependencyInjection;
using CarvaoControl.Domain.Interfaces;
using CarvaoControl.Infrastructure.Data.Sqlite;

namespace CarvaoControl.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string dbPath)
        {
            services.AddSingleton(new SqliteContext(dbPath));
            services.AddScoped<IProdutoRepository, SqliteProdutoRepository>();
            services.AddScoped<IVendaRepository, SqliteVendaRepository>();

            return services;
        }
    }
}