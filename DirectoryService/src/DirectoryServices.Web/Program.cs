using DirectoryServices.Infrastructure.Postgres;
using DirectoryServices.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "swagger test"));
}

app.MapControllers();

app.Run();
