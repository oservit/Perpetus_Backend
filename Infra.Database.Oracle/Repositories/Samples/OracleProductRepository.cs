using Infra.Database.Abstractions.Interfaces;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using System.Data;
using Core.Domain.Samples.Interfaces;
using Core.Domain.Samples.Entities;

namespace Infra.Database.Oracle.Repositories.Samples;

public class OracleProductRepository : IProductRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OracleProductRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public virtual string ConnectionName
    => "OracleDefault";

    private IDbConnection CreateConnection()
        => _connectionFactory.CreateConnection();

    // =========================================================
    // GET BY ID
    // =========================================================
    public async Task<Product?> GetByIdAsync(object id)
    {
        using var conn = CreateConnection();

        var sql = @"
            SELECT 
                id            AS Id,
                name          AS Name,
                description   AS Description,
                category      AS Category,
                unit_price    AS UnitPrice,
                stock_quantity AS StockQuantity,
                unit_of_measure AS UnitOfMeasure,
                manufacturer  AS Manufacturer,
                created_at    AS CreatedAt
            FROM products
            WHERE id = :Id";

        return await conn.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }

    // =========================================================
    // GET ALL
    // =========================================================
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        using var conn = CreateConnection();

        var sql = @"
            SELECT 
                id            AS Id,
                name          AS Name,
                description   AS Description,
                category      AS Category,
                unit_price    AS UnitPrice,
                stock_quantity AS StockQuantity,
                unit_of_measure AS UnitOfMeasure,
                manufacturer  AS Manufacturer,
                created_at    AS CreatedAt
            FROM products";

        return await conn.QueryAsync<Product>(sql);
    }

    // =========================================================
    // INSERT (Oracle SAFE com RETURNING INTO)
    // =========================================================
    public async Task<long> InsertAsync(Product entity)
    {
        using var conn = CreateConnection();

        var sql = @"
            INSERT INTO products
            (
                name,
                description,
                category,
                unit_price,
                stock_quantity,
                unit_of_measure,
                manufacturer,
                created_at
            )
            VALUES
            (
                :Name,
                :Description,
                :Category,
                :UnitPrice,
                :StockQuantity,
                :UnitOfMeasure,
                :Manufacturer,
                :CreatedAt
            )
            RETURNING id INTO :Id";

        var parameters = new DynamicParameters();

        parameters.Add("Name", entity.Name);
        parameters.Add("Description", entity.Description);
        parameters.Add("Category", entity.Category);
        parameters.Add("UnitPrice", entity.UnitPrice);
        parameters.Add("StockQuantity", entity.StockQuantity);
        parameters.Add("UnitOfMeasure", entity.UnitOfMeasure);
        parameters.Add("Manufacturer", entity.Manufacturer);
        parameters.Add("CreatedAt", entity.CreatedAt);

        parameters.Add(
            name: "Id",
            dbType: DbType.Int64,
            direction: ParameterDirection.Output
        );

        await conn.ExecuteAsync(sql, parameters);

        var id = parameters.Get<long>("Id");

        entity.Id = id;

        return id;
    }

    // =========================================================
    // UPDATE
    // =========================================================
    public async Task<int> UpdateAsync(Product entity)
    {
        using var conn = CreateConnection();

        var sql = @"
            UPDATE products
            SET 
                name = :Name,
                description = :Description,
                category = :Category,
                unit_price = :UnitPrice,
                stock_quantity = :StockQuantity,
                unit_of_measure = :UnitOfMeasure,
                manufacturer = :Manufacturer
            WHERE id = :Id";

        return await conn.ExecuteAsync(sql, new
        {
            entity.Name,
            entity.Description,
            entity.Category,
            entity.UnitPrice,
            entity.StockQuantity,
            entity.UnitOfMeasure,
            entity.Manufacturer,
            entity.Id
        });
    }

    // =========================================================
    // DELETE
    // =========================================================
    public async Task<int> DeleteAsync(object id)
    {
        using var conn = CreateConnection();

        var sql = @"DELETE FROM products WHERE id = :Id";

        return await conn.ExecuteAsync(sql, new { Id = id });
    }
}