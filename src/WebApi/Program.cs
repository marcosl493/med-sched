using Infrastructure;
using Scalar.AspNetCore;
using WebApi.Endpoints;
using WebApi.Extensions;
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services
    .AddOpenApi()
    .AddInfrastructure(builder.Configuration)
    .AddAuth(builder.Configuration)
    .AddMediator();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection()
   .UseAuthentication()
   .UseAuthorization()
   .UseRateLimiter();
app.MapAuthEndpoints();
app.MapPatientEndpoints();
app.Run();

