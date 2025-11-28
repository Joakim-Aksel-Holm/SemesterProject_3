using System.Globalization;
using CsvHelper;
using Npgsql;

namespace BeerProduction.Services;

public class BatchAnalysisRecord
{
    public DateTime Timestamp { get; set; }
    public int MachineId { get; set; }
    public int BatchId { get; set; }
    public string BeerType { get; set; }
    public int ProducedAmount { get; set; }
    public int DefectCount { get; set; }
    public double DefectRate { get; set; }
    public int MachineSpeed { get; set; } // Products per minute
}

public class BatchAnalysisService
{
    private readonly DatabaseConnection _dbConnection;

    public BatchAnalysisService(DatabaseConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task LogBatchDataAsync(MachineControlService machineControlService)
    {
        try
        {

            // DEBUG: 
            Console.WriteLine("=== BATCH ANALYSIS DEBUG ===");
            Console.WriteLine($"MachineId: {machineControlService.GetMachineId()}");
            Console.WriteLine($"BatchId: {machineControlService.GetBatchId()}");
            Console.WriteLine($"Produced: {machineControlService.GetProduced()}");
            Console.WriteLine($"Defects: {machineControlService.GetDefects()}");
            Console.WriteLine($"PPM: {machineControlService.GetPpm()}");
            Console.WriteLine($"BeerType: {machineControlService.GetCurrentBatch()}");
            Console.WriteLine("============================");

            var record = new BatchAnalysisRecord
            {
                Timestamp = DateTime.Now,
                MachineId = machineControlService.GetMachineId(),
                BatchId = machineControlService.GetBatchId(),
                BeerType = machineControlService.GetCurrentBatch().ToString(),
                ProducedAmount = machineControlService.GetProduced(),
                DefectCount = machineControlService.GetDefects(),
                DefectRate = await machineControlService.GetDefectRateAsync(),
                MachineSpeed = machineControlService.GetPpm()
            };
            await using var conn = await _dbConnection.OpenAsync();
            const string sql = @"
            INSERT INTO batch_analysis (timestamp, machine_id, batch_id, beer_type, produced_amount, defect_count, defect_rate, machine_speed)
            VALUES (@timestamp, @machineId, @batchId, @beerType, @producedAmount, @defectCount, @defectRate, @machineSpeed)";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("timestamp", record.Timestamp);
            cmd.Parameters.AddWithValue("machineId", record.MachineId);
            cmd.Parameters.AddWithValue("batchId", record.BatchId);
            cmd.Parameters.AddWithValue("beerType", record.BeerType);
            cmd.Parameters.AddWithValue("producedAmount", record.ProducedAmount);
            cmd.Parameters.AddWithValue("defectCount", record.DefectCount);
            cmd.Parameters.AddWithValue("defectRate", record.DefectRate);
            cmd.Parameters.AddWithValue("machineSpeed", record.MachineSpeed);

            await cmd.ExecuteNonQueryAsync();
            Console.WriteLine("Data logged successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error logging batch data: {ex.Message}");
        }
    }

    public async Task<List<BatchAnalysisRecord>> GetBatchDataAsync(
        DateTime startDate,
        DateTime endDate,
        int? machineId = null)
    {
        await using var conn = await _dbConnection.OpenAsync();

        var sql = @"
            SELECT timestamp, machine_id, batch_id, beer_type, produced_amount, defect_count, defect_rate, machine_speed
            FROM batch_analysis 
            WHERE timestamp >= @startDate AND timestamp <= @endDate";
        if (machineId.HasValue)
        {
            sql += " AND machine_id = @machineId";
        }
        
        sql += " ORDER BY timestamp";
        
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("startDate", startDate);
        cmd.Parameters.AddWithValue("endDate", endDate);
        
        if (machineId.HasValue)
        {
            cmd.Parameters.AddWithValue("machineId", machineId.Value);
        }
        
        await using var reader = await cmd.ExecuteReaderAsync();
        var results = new List<BatchAnalysisRecord>();
        while (await reader.ReadAsync())
        {
            results.Add(new BatchAnalysisRecord
            {
                Timestamp = reader.GetDateTime(0),
                MachineId = reader.GetInt32(1),
                BatchId = reader.GetInt32(2),
                BeerType = reader.GetString(3),
                ProducedAmount = reader.GetInt32(4),
                DefectCount = reader.GetInt32(5),
                DefectRate = reader.GetDouble(6),
                MachineSpeed = reader.GetInt32(7)
            });
        }
        
        return results;
    }
    
    public async Task<string> ExportToCsvAsync(DateTime startDate, DateTime endDate, int? machineId = null)
    {
        var data = await GetBatchDataAsync(startDate, endDate, machineId);
        
        // Create Exports directory if it doesn't exist
        var exportsDir = Path.Combine(Directory.GetCurrentDirectory(), "Exports");
        if (!Directory.Exists(exportsDir))
        {
            Directory.CreateDirectory(exportsDir);
        }
        
        var fileName = $"batch_analysis_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}_{DateTime.Now:HHmmss}.csv";
        var filePath = Path.Combine(exportsDir, fileName);
        
        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            await csv.WriteRecordsAsync(data);
        }
        
        return filePath;
    }
}
