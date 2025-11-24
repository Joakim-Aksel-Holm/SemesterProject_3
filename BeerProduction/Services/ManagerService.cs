using System.Data;
using BeerProduction.Components.Model;
using Microsoft.AspNetCore.Authorization;
using Npgsql;

namespace BeerProduction.Services;

[Authorize(Roles = "Manager")]
public class ManagerService
{
    private DatabaseConnection _db;

    public BatchQueue batchQueue;
    public ManagerService(DatabaseConnection db)
    {
        _db = db;
    }
    
    //Manager-specific methods :
    public async Task<List<MachineControlService>> GetAllMachinesAsync()
    {
        //Sql queries specific to manager dashboard

        var machines = new List<MachineControlService>();
            
        await using var conn = await _db.OpenAsync();   // pooled under the hood
        await using var cmd  = conn.CreateCommand();
        
        cmd.CommandText = @"SELECT id, url, name
                            FROM machine_table;
                        ";
        
        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var machineId = reader.GetInt32(0);
            var machineUrl = reader.GetString(1);
            var machineName = reader.GetString(2);
            
            var machine = new MachineControl(machineId, machineUrl, machineName);
            var machineService = new MachineControlService(machine, batchQueue);

            machines.Add(machineService);
        }

        return machines;
    }
    public async Task<int> GetTotalCompletedBatchesAsync()
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = conn.CreateCommand();
    
        cmd.CommandText = "SELECT COUNT(*) FROM batch_history WHERE status = 'Completed'";
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<int> GetTotalProductionLitresAsync()
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = conn.CreateCommand();
    
        cmd.CommandText = "SELECT COALESCE(SUM(amount_liters), 0) FROM batch_history WHERE status = 'Completed'";
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
}