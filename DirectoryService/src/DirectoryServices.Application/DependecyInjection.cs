using DirectoryServices.Application.Locations;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryServices.Application
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ILocationsService, LocationsService>();

            return services;
        }
    }
}
