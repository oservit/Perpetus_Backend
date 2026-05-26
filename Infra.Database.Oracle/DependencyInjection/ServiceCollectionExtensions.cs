using Core.Domain.Interfaces.Samples;

using Infra.Database.Abstractions.Interfaces;

using Infra.Database.Oracle.Factories;
using Infra.Database.Oracle.Repositories.Samples;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace Infra.Database.Oracle.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOracleInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDbConnectionFactory,
            OracleConnectionFactory>();

        services.AddScoped<IProductRepository,
            OracleProductRepository>();

        return services;
    }
}