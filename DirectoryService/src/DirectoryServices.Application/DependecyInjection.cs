using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Departaments.Services.OldDepsDeletionService;
using DirectoryServices.Application.Locations.Queries.GetLocationById;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static CSharpFunctionalExtensions.Result;

namespace DirectoryServices.Application
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(typeof(DependecyInjection).Assembly);

            var assembly = typeof(DependecyInjection).Assembly;

            services.Scan(scan => scan.FromAssemblies(assembly)
                .AddClasses(classes => classes
                    .AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>), typeof(IQueryHandler<,>))) // сам зарегистрирует в DI все command и query хэндлеры (реализации интерфейса)
                .AsSelfWithInterfaces()
                .WithScopedLifetime());

            services.AddScoped<IDeletionService, OldDepsDeletionService>();

            services.AddStackExchangeRedisCache(setup =>
            {
                setup.Configuration = configuration.GetConnectionString("Redis");
            });

            services.AddHybridCache(options =>
            {
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    LocalCacheExpiration = configuration.GetValue<TimeSpan>("CacheOptions:InMemMins"), // in memory 1 lvl
                    Expiration = configuration.GetValue<TimeSpan>("CacheOptions:InRedisMins"), // 2nd lvl
                };
            });

            return services;
        }
    }
}
