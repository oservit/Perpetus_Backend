using Core.Application.DependencyInjection;

using Infra.Database.Abstractions.Configuration;
using Infra.Database.PostgreSql.DependencyInjection;

using Infra.Hosting.DependencyInjection;
using Infra.Hosting.Options;

using Infra.Messaging.RabbitMQ.DependencyInjection;

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
// MODULES
// ----------------------------------------------------

builder.Services
    .AddApiSwagger()
    .AddApplication()
    .AddPostgreSqlInfrastructure(
        builder.Configuration)
    .AddRabbitMqMessaging(
        builder.Configuration)
    .AddApiAuthentication(
        builder.Configuration)
    .AddApiCors(
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