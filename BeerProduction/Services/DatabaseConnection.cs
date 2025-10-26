// BeerProduction/Services/DatabaseConnection.cs

using Npgsql;

namespace BeerProduction.Services;

public class DatabaseConnection
{
    private readonly string _cs;

    public DatabaseConnection(IConfiguration cfg)
    {
        // Pulls the value from Configuration["ConnectionStrings:Default"]
        // which is built from appsettings.json → appsettings.Development.json
        // → appsettings.Development.Local.json (last wins)
        _cs = cfg.GetConnectionString("Default")
              ?? throw new InvalidOperationException("Missing ConnectionStrings:Default");
    }

    /// <summary>Get a new logical connection (pooled under the hood). Caller must dispose.</summary>>
    public async Task<NpgsqlConnection> OpenAsync(CancellationToken cancellationToken = default)
    {
        var conn = new NpgsqlConnection(_cs);
        await conn.OpenAsync(cancellationToken);
        return conn;
    }

    // * Npgsql uses pooling under the hood already by default through the Npgsql package as long as callers dispose the returned connection
    // * So we don't have to make our own pooling process.
}