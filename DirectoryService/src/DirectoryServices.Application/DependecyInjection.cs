using DirectoryServices.Application.Locations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryServices.Application
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ILocationsService, LocationsService>();

            services.AddValidatorsFromAssembly(typeof(DependecyInjection).Assembly);

            return services;
        }
    }
}
