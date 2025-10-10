using DirectoryServices.Application;
using DirectoryServices.Infrastructure.Postgres;

namespace DirectoryServices.Web
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddProgramDependencies(this IServiceCollection services)
        {
            services.AddWebDependencies();
            services.AddApplication();

            return services;
        }

        public static IServiceCollection AddWebDependencies(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddOpenApi();

            return services;
        }
    }
}
