using Infra.Database.Abstractions.Configuration;
using Infra.Database.PostgreSql.DependencyInjection;

using Infra.Messaging.RabbitMQ.DependencyInjection;

using Workers.LedgerWriter;
using Workers.LedgerWriter.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<DatabaseConnectionsOptions>(
    builder.Configuration.GetSection("DatabaseConnections"));

builder.Services
    .AddPostgreSqlInfrastructure(builder.Configuration)
    .AddRabbitMqMessaging(builder.Configuration)
    .AddConsumers()
    .AddHostedService<Worker>();

var host = builder.Build();

host.Run();