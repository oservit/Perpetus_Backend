using System.Data;

namespace Infra.Database.Abstractions.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection(string connectionName = "Default");
}