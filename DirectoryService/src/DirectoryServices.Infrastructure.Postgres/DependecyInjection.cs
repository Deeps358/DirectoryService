using DirectoryServices.Application.Database;
using DirectoryServices.Application.Departaments;
using DirectoryServices.Application.Locations;
using DirectoryServices.Application.Positions;
using DirectoryServices.Infrastructure.Postgres.Database;
using DirectoryServices.Infrastructure.Postgres.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryServices.Infrastructure.Postgres
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<DirectoryServiceDbContext>(_ =>
                new DirectoryServiceDbContext(configuration.GetConnectionString("DirectoryServiceDevDb")!));

            services.AddScoped<ILocationsRepository, LocationsRepository>();
            services.AddScoped<IDepartamentsRepository, DepartamentsRepository>();
            services.AddScoped<IPositionsRepository, PositionsRepository>();

            services.AddScoped<ITransactionManager, TransactionManager>();

            return services;
        }
    }
}
