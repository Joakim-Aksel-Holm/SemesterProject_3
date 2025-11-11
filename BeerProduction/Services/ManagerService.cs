using System.Data;
using BeerProduction.Components.Model;
using Microsoft.AspNetCore.Authorization;
using Npgsql;

namespace BeerProduction.Services;

[Authorize(Roles = "Manager")]

public class ManagerService
{
    private readonly string? _connectionString;
    
    public ManagerService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default");    }
    
    //Manager-specific methods :

    public async Task<List<Machine>> GetAllMachinesAsync()
    {
        //Sql queries specific to manager dashboard
        
        var machines = new List<Machine>();
        await using var connection = new NpgsqlConnection(_connectionString); 
        await connection.OpenAsync();

        var sql = @"SELECT machine_id, machine_url, name, status, last_maintenance, location FROM machine_table";

        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var machine = new Machine
            {
                MachineId = reader.GetInt32("machine_id"),
                MachineUrl = reader.GetString("machine_url"),
                Name = reader.GetString("name"),
                Status = reader.GetString("status"),
                LastMaintenance = reader.GetDateTime("last_maintenance"),
                Location = reader.IsDBNull("location") ? "" :reader.GetString("location")
            };
            machines.Add(machine);
        }

        return machines;
    }
}