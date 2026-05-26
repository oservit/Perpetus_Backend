using Core.Application.DependencyInjection;

using Infra.Database.Abstractions.Configuration;
using Infra.Database.PostgreSql.DependencyInjection;

using Infra.Hosting.DependencyInjection;
using Infra.Hosting.Options;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// OPTIONS
// ----------------------------------------------------

builder.Services.Configure<DatabaseConnectionsOptions>(
    builder.Configuration.GetSection("DatabaseConnections"));

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.Configure<CorsOptions>(
    builder.Configuration.GetSection("Cors"));

// ----------------------------------------------------
// CONTROLLERS
// ----------------------------------------------------

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ----------------------------------------------------
// CUSTOM MODULES
// ----------------------------------------------------

builder.Services.AddApiSwagger();

builder.Services.AddApplication();

builder.Services.AddPostgreSqlInfrastructure(
    builder.Configuration);

builder.Services.AddApiAuthentication(
    builder.Configuration);

builder.Services.AddApiCors(
    builder.Configuration);

var app = builder.Build();

// ----------------------------------------------------
// PIPELINE
// ----------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseCors("Default");

app.UseAuthentication();

app.UseAuthorization();

app.MapGet("/", () =>
    Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.MapControllers();

await app.RunAsync();