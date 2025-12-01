namespace BeerProduction.Services;

public class ManagerService
{
    private DatabaseConnection _db;
    private bool _machinesInitilized;
    private List<MachineControlService> _cachedMachines = new();
    private readonly object _lock = new();

    public ManagerService(DatabaseConnection db)
    {
        _db = db;
    }
    
    //Manager-specific methods :
    /// <summary>
    /// Returns all machines in the database
    /// </summary>
    public async Task<List<MachineControlService>> GetAllMachinesAsync()
    {
        //Sql queries specific to manager dashboard

        if (_machinesInitilized) return _cachedMachines;
        
        lock (_lock) if (_machinesInitilized) return _cachedMachines;
        
        var machines = new List<MachineControl>();
            
        await using var conn = await _db.OpenAsync();   // pooled under the hood
        await using var cmd  = conn.CreateCommand();
        
        cmd.CommandText = @"SELECT id, url, name
                            FROM machine_table;
                        ";
        
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            machines.Add(new MachineControl(
                reader.GetInt32(0),
                reader.GetString(1), 
                reader.GetString(2)
            ));
        }

        var connectionTasks = machines.Select(m => m.TryConnectAsync()).ToList();
        await Task.WhenAll(connectionTasks);
        
        _cachedMachines = machines.Select(m => new MachineControlService(m)).ToList();
        
        _machinesInitilized = true;
        
        return _cachedMachines;
    }
    
    /// <summary>
    /// Returns the total number of completed batches
    /// </summary>
    public async Task<int> GetTotalCompletedBatchesAsync()
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = conn.CreateCommand();
    
        cmd.CommandText = "SELECT COUNT(*) FROM batch_history WHERE status = 'Completed'";
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    /// <summary>
    /// Returns the total amount of produced beer litres
    /// </summary>
    public async Task<int> GetTotalProductionLitresAsync()
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = conn.CreateCommand();
    
        cmd.CommandText = "SELECT COALESCE(SUM(amount_liters), 0) FROM batch_history WHERE status = 'Completed'";
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task AlertRead()
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = conn.CreateCommand();
    
        cmd.CommandText = "SELECT FROM alert_table(alert_id, user_id, call_time)";
        var result = await cmd.ExecuteScalarAsync();
        
    } 
    
}