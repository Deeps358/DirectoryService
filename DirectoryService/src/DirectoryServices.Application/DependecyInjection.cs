using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Locations.Queries.GetLocationById;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryServices.Application
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(DependecyInjection).Assembly);

            var assembly = typeof(DependecyInjection).Assembly;

            services.Scan(scan => scan.FromAssemblies(assembly)
                .AddClasses(classes => classes
                    .AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>))) // сам зарегистрирует в DI все command хэндлеры (реализации интерфейса)
                .AsSelfWithInterfaces()
                .WithScopedLifetime());

            services.Scan(scan => scan.FromAssemblies(assembly)
                .AddClasses(classes => classes
                    .AssignableToAny(typeof(IQueryHandler<,>))) // сам зарегистрирует в DI все query хэндлеры (реализации интерфейса)
                .AsSelfWithInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}
