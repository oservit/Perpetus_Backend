using Core.Application.Samples.DTOs;
using Core.Application.Samples.Services;
using Core.Domain.Interfaces.Samples;
using Infra.Database.Abstractions.Configuration;
using Infra.Database.Abstractions.Interfaces;
using Infra.Database.Oracle.Factories;
using Infra.Database.Oracle.Repositories.Samples;
using Infra.Database.PostgreSql.Factories;
using Infra.Database.PostgreSql.Repositories.Samples;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Bootstrap Console (Multi-SGBD Runner) ===");

        var configuration = BuildConfiguration();

        
        Console.WriteLine("\n==============================");
        Console.WriteLine("EXECUTANDO POSTGRESQL FLOW");
        Console.WriteLine("==============================\n");

        await RunPostgres(configuration);
        
        Console.WriteLine("\n==============================");
        Console.WriteLine("EXECUTANDO ORACLE FLOW");
        Console.WriteLine("==============================\n");

        await RunOracle(configuration);

        Console.WriteLine("\n=== EXECUÇÃO FINALIZADA ===");
    }

    static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    static async Task RunPostgres(IConfiguration configuration)
    {
        var sp = BuildPostgresServiceProvider(configuration);

        var productService = sp.GetRequiredService<ProductService>();

        

        Console.WriteLine("Inserindo produto no PostgreSQL...");

        var dto = CreateSampleProduct();

        var id = await productService.InsertAsync(dto);

        Console.WriteLine($"[POSTGRES] ID gerado: {id}");

        

        Console.WriteLine("\nListando produtos PostgreSQL...");

        var products = await productService.GetAllAsync();

        foreach (var p in products)
        {
            Console.WriteLine($"[POSTGRES] {p.Id} | {p.Name} | {p.UnitPrice}");
        }
    }

    static async Task RunOracle(IConfiguration configuration)
    {
        var sp = BuildOracleServiceProvider(configuration);

        var productService = sp.GetRequiredService<ProductService>();

        
        Console.WriteLine("Inserindo produto no Oracle...");

        var dto = CreateSampleProduct();

        var id = await productService.InsertAsync(dto);

        Console.WriteLine($"[ORACLE] ID gerado: {id}");
        

        Console.WriteLine("\nListando produtos Oracle...");

        var products = await productService.GetAllAsync();

        foreach (var p in products)
        {
            Console.WriteLine($"[ORACLE] {p.Id} | {p.Name} | {p.UnitPrice}");
        }
    }

    static ServiceProvider BuildPostgresServiceProvider(IConfiguration configuration)
    {
        var services = new ServiceCollection();

        services.Configure<DatabaseConnectionsOptions>(
            configuration.GetSection("DatabaseConnections"));

        services.AddScoped<IDbConnectionFactory, PostgreSqlConnectionFactory>();
        services.AddScoped<IProductRepository, PostgreSqlProductRepository>();
        services.AddScoped<ProductService>();

        return services.BuildServiceProvider();
    }

    static ServiceProvider BuildOracleServiceProvider(IConfiguration configuration)
    {
        var services = new ServiceCollection();

        services.Configure<DatabaseConnectionsOptions>(
            configuration.GetSection("DatabaseConnections"));

        services.AddScoped<IDbConnectionFactory, OracleConnectionFactory>();
        services.AddScoped<IProductRepository, OracleProductRepository>();
        services.AddScoped<ProductService>();

        return services.BuildServiceProvider();
    }

    static CreateProductDto CreateSampleProduct()
    {
        return new CreateProductDto
        {
            Name = "Teclado Mecânico RGB",
            Description = "Teclado mecânico switch blue ABNT2",
            Category = "Periféricos",
            UnitPrice = 249.90m,
            StockQuantity = 15,
            UnitOfMeasure = "UN",
            Manufacturer = "Logitech"
        };
    }
}