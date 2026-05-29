using Core.Domain.Samples.Interfaces;
using Infra.Database.Abstractions.Interfaces;

using Infra.Database.PostgreSql.Factories;
using Infra.Database.PostgreSql.Repositories.Samples;
using Infra.Database.PostgreSql.Repositories.Logs;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Domain.Logs.Interfaces;

namespace Infra.Database.PostgreSql.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgreSqlInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDbConnectionFactory,PostgreSqlConnectionFactory>();

        services.AddScoped<IProductRepository,PostgreSqlProductRepository>();

        services.AddScoped<IEventInboxRepository,EventInboxRepository>();

        return services;
    }
}