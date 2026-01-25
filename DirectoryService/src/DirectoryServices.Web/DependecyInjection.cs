using DirectoryServices.Application;
using Shared.EndpointResult;
using Shared.ResultPattern;

namespace DirectoryServices.Web
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddProgramDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddWebDependencies();
            services.AddApplication(configuration);

            return services;
        }

        public static IServiceCollection AddWebDependencies(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddOpenApi(options =>
            {
                options.AddSchemaTransformer((schema, context, _) =>
                {
                    if (context.JsonTypeInfo.Type == typeof(Envelope<Error>))
                    {
                        if (schema.Properties.TryGetValue("error", out var errorsProp))
                        {
                            errorsProp.Items.Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.Schema,
                                Id = "Error"
                            };
                        }
                    }

                    return Task.CompletedTask;
                });
            });

            return services;
        }
    }
}
