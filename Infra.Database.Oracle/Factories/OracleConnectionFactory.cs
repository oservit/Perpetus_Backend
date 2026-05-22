using System;
using System.Data;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using Infra.Database.Abstractions.Configuration;
using Infra.Database.Abstractions.Interfaces;

namespace Infra.Database.Oracle.Factories;

public class OracleConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseConnectionsOptions _options;

    public OracleConnectionFactory(IOptions<DatabaseConnectionsOptions> options)
    {
        _options = options.Value;
    }

    public IDbConnection CreateConnection(string connectionName = "Default")
    {
        if (!_options.TryGetValue(connectionName, out var config))
            throw new ArgumentException($"A conexão nomeada '{connectionName}' não foi encontrada no appsettings.json.");

        if (!config.Sgbd.Equals("oracle", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"[ERRO DE ARQUITETURA] O repositório solicitou uma conexão Oracle, " +
                $"mas a conexão '{connectionName}' no JSON está configurada como '{config.Sgbd}'.");
        }

        var serviceNameOrSid = !string.IsNullOrEmpty(config.Sid) ? $"SID={config.Sid}" : $"SERVICE_NAME={config.Schema}";
        var connectionString = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={config.Host})(PORT={config.Port}))(CONNECT_DATA=({serviceNameOrSid})));User Id={config.User};Password={config.Password};";

        var conn = new OracleConnection(connectionString);
        conn.BindByName = true;
        return conn;
    }
}