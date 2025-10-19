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

    /// Get a NEW open connection (caller must dispose).
    public NpgsqlConnection Open()
    {
        var conn = new NpgsqlConnection(_cs);
        conn.Open();                 // Npgsql uses pooling under the hood
        return conn;
    }
}