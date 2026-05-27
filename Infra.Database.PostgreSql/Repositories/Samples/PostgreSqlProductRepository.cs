using Core.Domain.Samples.Entities;
using Core.Domain.Samples.Interfaces;
using Infra.Database.Abstractions.Interfaces;
using Infra.Database.PostgreSql.Repositories.Common;

namespace Infra.Database.PostgreSql.Repositories.Samples;

public class PostgreSqlProductRepository
    : PostgreSqlBaseRepository<Product>,
      IProductRepository
{
    public PostgreSqlProductRepository(
        IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    // Caso queira usar outra conexão:
    // public override string ConnectionName
    //     => "PostgreSqlConnection2";

    // Caso queira outro schema:
    // protected override string Schema
    //     => "custom_schema";
}