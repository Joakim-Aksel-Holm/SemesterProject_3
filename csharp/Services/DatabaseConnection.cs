using System;
using System.Data;
using Npgsql;

namespace SemesterProject_3.Services;

public class DatabaseConnection : IDisposable
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;

    public DatabaseConnection()
    {
        string host = "147.185.221.211";
        int port = 7617;
        string database = "semesterprojekt3db";
        string username = "remote_user";
        string password = "X3Kd0f38dbaFdFto";

        _connectionString =
            $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
    }

    public NpgsqlConnection GetConnection()
    {
        if (_connection == null)
            _connection = new NpgsqlConnection(_connectionString);

        if (_connection.State != ConnectionState.Open)
            _connection.Open();

        return _connection;
    }

    public void Dispose()
    {
        if (_connection != null && _connection.State == ConnectionState.Open)
            _connection.Close();
    }
}