using Core.Application.DependencyInjection;

using Core.Application.Samples.DTOs;
using Core.Application.Samples.Services;

using Infra.Database.Abstractions.Configuration;
using Infra.Database.PostgreSql.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Bootstrap Console ===");

        var configuration = BuildConfiguration();

        var serviceProvider =
            BuildServiceProvider(configuration);

        using var scope =
            serviceProvider.CreateScope();

        var productService =
            scope.ServiceProvider
                .GetRequiredService<ProductService>();

        Console.WriteLine("\nListando produtos...\n");

        var products =
            await productService.GetAllAsync();

        foreach (var p in products)
        {
            Console.WriteLine(
                $"{p.Id} | {p.Name} | {p.UnitPrice}");
        }

        Console.WriteLine("\n=== EXECUÇÃO FINALIZADA ===");
    }

    static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: true)
            .Build();
    }

    static ServiceProvider BuildServiceProvider(
        IConfiguration configuration)
    {
        var services =
            new ServiceCollection();

        // ----------------------------------------------------
        // OPTIONS
        // ----------------------------------------------------

        services.Configure<DatabaseConnectionsOptions>(
            configuration.GetSection("DatabaseConnections"));

        // ----------------------------------------------------
        // MODULES
        // ----------------------------------------------------

        services.AddApplication();

        services.AddPostgreSqlInfrastructure(
            configuration);

        return services.BuildServiceProvider();
    }
}