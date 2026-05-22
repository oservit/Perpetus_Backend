using Core.Domain.Interfaces.Common;
using Dommel;
using Infra.Database.Abstractions.Interfaces;
using System.Data;

namespace Infra.Database.PostgreSql.Repositories.Common;

public abstract class PostgreSqlBaseRepository<TEntity>
    : IBaseRepository<TEntity>
    where TEntity : class
{
    protected readonly IDbConnectionFactory ConnectionFactory;

    protected PostgreSqlBaseRepository(
        IDbConnectionFactory connectionFactory)
    {
        ConnectionFactory = connectionFactory;
    }

    public virtual string ConnectionName
        => "PostgreSqlDefault";

    /// <summary>
    /// Schema padrão do PostgreSQL.
    /// </summary>
    protected virtual string Schema
        => "public";

    protected IDbConnection CreateConnection()
    {
        return ConnectionFactory.CreateConnection(ConnectionName);
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        object id)
    {
        using var conn = CreateConnection();

        return await conn.GetAsync<TEntity>(id);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        using var conn = CreateConnection();

        return await conn.GetAllAsync<TEntity>();
    }

    public virtual async Task<long> InsertAsync(
        TEntity entity)
    {
        using var conn = CreateConnection();

        var result = await conn.InsertAsync(entity);

        return Convert.ToInt64(result);
    }

    public virtual async Task<int> UpdateAsync(
        TEntity entity)
    {
        using var conn = CreateConnection();

        var updated = await conn.UpdateAsync(entity);

        return updated ? 1 : 0;
    }

    public virtual async Task<int> DeleteAsync(
        object id)
    {
        using var conn = CreateConnection();

        var entity = await conn.GetAsync<TEntity>(id);

        if (entity is null)
            return 0;

        var deleted = await conn.DeleteAsync(entity);

        return deleted ? 1 : 0;
    }

    /// <summary>
    /// Retorna o nome fully-qualified da tabela.
    /// Ex: public.products
    /// </summary>
    protected string Table(string tableName)
    {
        return $"{Schema}.{tableName}";
    }
}