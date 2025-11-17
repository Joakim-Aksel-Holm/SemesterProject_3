using System.Data;
using BeerProduction.Components.Model;
using Microsoft.AspNetCore.Authorization;
using Npgsql;

namespace BeerProduction.Services;

[Authorize(Roles = "Manager")]
public class ManagerService
{
    private DatabaseConnection Db;

    //Manager-specific methods :

    public async Task<List<MachineControlService>> GetAllMachinesAsync()
    {
        //Sql queries specific to manager dashboard

        var machines = new List<MachineControlService>();
            
        await using var conn = await Db.OpenAsync();   // pooled under the hood
        await using var cmd  = conn.CreateCommand();
        
        cmd.CommandText = @"SELECT machine_id, machine_url
                            FROM machine_table;
                        ";
        
        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var machineId = reader.GetInt32(0);
            var machineUrl = reader.GetString(1);
            
            var machine = new MachineControl(machineId, machineUrl);
            var machineService = new MachineControlService(machine);

            machines.Add(machineService);
        }

        return machines;
    }
}