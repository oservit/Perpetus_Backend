using System;
using System.Data;
using Infra.Database.Abstractions.Configuration;
using Infra.Database.Abstractions.Interfaces;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infra.Database.PostgreSql.Factories;

public class PostgreSqlConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseConnectionsOptions _options;

    public PostgreSqlConnectionFactory(
        IOptions<DatabaseConnectionsOptions> options)
    {
        _options = options.Value;
    }

    public IDbConnection CreateConnection(
        string connectionName = "Default")
    {
        if (!_options.TryGetValue(connectionName, out var config))
        {
            throw new ArgumentException(
                $"A conexão nomeada '{connectionName}' não foi encontrada no appsettings.json.");
        }

        if (!config.Sgbd.Equals(
                "postgres",
                StringComparison.OrdinalIgnoreCase)
            &&
            !config.Sgbd.Equals(
                "postgresql",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"[ERRO DE ARQUITETURA] O repositório solicitou uma conexão PostgreSQL, " +
                $"mas a conexão '{connectionName}' no JSON está configurada como '{config.Sgbd}'.");
        }

        var connectionString =
            $"Host={config.Host};" +
            $"Port={config.Port};" +
            $"Database={config.Database};" +
            $"Username={config.User};" +
            $"Password={config.Password};";

        return new NpgsqlConnection(connectionString);
    }
}