namespace Infra.Database.Abstractions.Configuration;

public class DatabaseConnectionsOptions : Dictionary<string, ConnectionDetails>
{
}

public class ConnectionDetails
{
    public string Sgbd { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string? Schema { get; set; }
    public string? Database { get; set; }
    public string? Sid { get; set; }
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}