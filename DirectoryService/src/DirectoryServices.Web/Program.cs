using DirectoryServices.Infrastructure.Postgres;
using DirectoryServices.Web;
using DirectoryServices.Web.Middlewares;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Debug()
    .WriteTo.Seq(builder.Configuration.GetConnectionString("Seq") ?? throw new ArgumentNullException("Seq"))
    .Enrich.WithThreadId()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentUserName()
    .Enrich.WithEnvironmentName()
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .CreateLogger();

builder.Services.AddProgramDependencies();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSerilog();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "swagger test"));

    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();

    await dbContext.Database.MigrateAsync();
}

app.MapControllers();

app.Run();

namespace DirectoryServices.Web
{
    // расширение для доступа Program из тестов
    public partial class Program;
}
