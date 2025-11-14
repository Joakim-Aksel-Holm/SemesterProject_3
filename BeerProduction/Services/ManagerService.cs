using System.Data;
using BeerProduction.Components.Model;
using Microsoft.AspNetCore.Authorization;
using Npgsql;

namespace BeerProduction.Services;

[Authorize(Roles = "Manager")]
public class ManagerService
{
    private readonly string? _connectionString;
    private readonly Random _random = new Random(); // For generating random data

    public ManagerService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default");
    }

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
                Location = reader.IsDBNull("location") ? "" : reader.GetString("location")
            };
            AddMockRealTimeData(machine);
            machines.Add(machine);
        }

        return machines;
    }

    private string GetRandomBeerType()
    {
        var beerTypes = new[] { "Pilsner", "Wheat", "IPA", "Stout", "Lager", "Ale" };
        return beerTypes[_random.Next(beerTypes.Length)];
    }

    private void AddMockRealTimeData(Machine machine)
    {
        // Simulate machine being online 80% of the time
        machine.IsOnline = _random.Next(0, 100) > 20;

        if (machine.IsOnline && machine.Status == "Operational")
        {
            // Machine is online and operational - generate realistic data
            machine.Temperature = (decimal)Math.Round(65 + (_random.NextDouble() * 10), 1); // 65-75°C
            machine.Pressure = Math.Round(1.5m + ((decimal)_random.NextDouble() * 0.5m), 1); // 1.5-2.0 bar
            machine.BatchProgress = _random.Next(0, 101); // 0-100%
            machine.ProductionRate = _random.Next(50, 150); // 50-150 units/min
            machine.DefectCount = _random.Next(0, 10);
            machine.AcceptableCount = _random.Next(100, 500);
            machine.CurrentBatch = GetRandomBeerType();
        }
        else
        {
            // Machine is offline or in maintenance - set to zero/default values
            machine.Temperature = 20.0m; // Room temperature
            machine.Pressure = 0;
            machine.BatchProgress = 0;
            machine.ProductionRate = 0;
            machine.DefectCount = 0;
            machine.AcceptableCount = 0;
            machine.CurrentBatch = "None";
        }
    }

    public void RefrechRealTimeData(List<Machine> machines)
    {
    }
}