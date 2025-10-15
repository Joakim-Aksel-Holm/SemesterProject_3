using System.Data;
using Npgsql;

namespace BeerProduction.Services;

public class DatabaseConnection : IDisposable
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;

    public DatabaseConnection()
    {
        // read the connection string from environment variables.
        // In docker compose we pass: ConnectionString_Default= ..

        var conn = Environment.GetEnvironmentVariable("ConnectionStrings__Default");

        if (string.IsNullOrEmpty(conn))
        {
            throw new InvalidOperationException(" Missing ConnectionStrings__Default environment variable." +
                                                "Create a .env.development file (for compose) or set the env var before running.");
        }

        _connectionString = conn;
    }

    public NpgsqlConnection GetConnection()
    {
        _connection ??= new NpgsqlConnection(_connectionString);

        if (_connection.State != ConnectionState.Open)
            _connection.Open();

        return _connection;
    }

    public void Dispose()
    {
        if (_connection is { State: ConnectionState.Open })
            _connection.Close();
    }
}