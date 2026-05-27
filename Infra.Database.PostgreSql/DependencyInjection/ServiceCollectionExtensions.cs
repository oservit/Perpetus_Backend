using Core.Domain.Samples.Interfaces;
using Infra.Database.Abstractions.Interfaces;

using Infra.Database.PostgreSql.Factories;
using Infra.Database.PostgreSql.Repositories.Samples;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Database.PostgreSql.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgreSqlInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDbConnectionFactory,
            PostgreSqlConnectionFactory>();

        services.AddScoped<IProductRepository,
            PostgreSqlProductRepository>();

        return services;
    }
}