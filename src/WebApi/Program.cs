using Infrastructure;
using Scalar.AspNetCore;
using WebApi.Endpoints;
using WebApi.Extensions;
var builder = WebApplication.CreateBuilder(args);

if(builder.Environment.EnvironmentName == "Staging")
    builder.WebHost.ConfigureKestrel(opt =>
    {
        opt.ListenAnyIP(8080);
    });
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services
    .AddOpenApi()
    .AddInfrastructure(builder.Configuration)
    .AddAuth(builder.Configuration)
    .AddMediator()
    .AddOutputCache();

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
    app.UseHttpsRedirection();


app.UseAuthentication()
   .UseAuthorization()
   .UseRateLimiter()
   .UseOutputCache();

app.MapAuthEndpoints();
app.MapPatientEndpoints();
app.MapPhysicianEndpoints();
app.MapScheduleEndpoints();
app.Run();

