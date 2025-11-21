using Npgsql;

namespace BeerProduction.Services;

public class ProductionTrackingService
{
    private readonly DatabaseConnection _db;

    public ProductionTrackingService(DatabaseConnection db)
    {
        _db = db;
    }

    // Record when a batch starts
    public async Task RecordBatchStartAsync(int machineId, int batchId, string beerType, int amountLiters)
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = conn.CreateCommand();
        
        cmd.CommandText = @"
            INSERT INTO batch_history (machine_id, batch_id, beer_type, amount_liters, acceptable_count, defect_count, status)
            VALUES (@machineId, @batchId, @beerType, @amountLiters, 0, 0, 'Running')
            ON CONFLICT (machine_id, batch_id) DO UPDATE SET
                beer_type = EXCLUDED.beer_type,
                amount_liters = EXCLUDED.amount_liters,
                start_time = CASE WHEN batch_history.start_time IS NULL THEN CURRENT_TIMESTAMP ELSE batch_history.start_time END";
        
        cmd.Parameters.AddWithValue("@machineId", machineId);
        cmd.Parameters.AddWithValue("@batchId", batchId);
        cmd.Parameters.AddWithValue("@beerType", beerType);
        cmd.Parameters.AddWithValue("@amountLiters", amountLiters);
        
        await cmd.ExecuteNonQueryAsync();
    }

    // Record when a batch completes
    public async Task RecordBatchCompletionAsync(int machineId, int batchId, int acceptableCount, int defectCount)
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = conn.CreateCommand();
        
        cmd.CommandText = @"
            UPDATE batch_history 
            SET status = 'Completed', 
                completion_time = CURRENT_TIMESTAMP,
                acceptable_count = @acceptableCount,
                defect_count = @defectCount
            WHERE machine_id = @machineId AND batch_id = @batchId";
        
        cmd.Parameters.AddWithValue("@machineId", machineId);
        cmd.Parameters.AddWithValue("@batchId", batchId);
        cmd.Parameters.AddWithValue("@acceptableCount", acceptableCount);
        cmd.Parameters.AddWithValue("@defectCount", defectCount);
        
        await cmd.ExecuteNonQueryAsync();
        
        // Update production summary
        await UpdateProductionSummaryAsync();
    }

    // Get total production statistics
    public async Task<ProductionStats> GetProductionStatsAsync()
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = conn.CreateCommand();
        
        cmd.CommandText = @"
            SELECT 
                COUNT(*) as total_batches,
                COALESCE(SUM(amount_liters), 0) as total_liters,
                COALESCE(SUM(acceptable_count), 0) as total_acceptable,
                COALESCE(SUM(defect_count), 0) as total_defects
            FROM batch_history 
            WHERE status = 'Completed'";
        
        await using var reader = await cmd.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return new ProductionStats
            {
                TotalBatchesCompleted = reader.GetInt32(0),
                TotalLitersProduced = reader.GetInt32(1),
                TotalAcceptableProducts = reader.GetInt32(2),
                TotalDefectProducts = reader.GetInt32(3)
            };
        }
        
        return new ProductionStats();
    }

    // Get today's production
    public async Task<ProductionStats> GetTodayProductionStatsAsync()
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = conn.CreateCommand();
        
        cmd.CommandText = @"
            SELECT 
                COUNT(*) as total_batches,
                COALESCE(SUM(amount_liters), 0) as total_liters,
                COALESCE(SUM(acceptable_count), 0) as total_acceptable,
                COALESCE(SUM(defect_count), 0) as total_defects
            FROM batch_history 
            WHERE status = 'Completed' AND DATE(completion_time) = CURRENT_DATE";
        
        await using var reader = await cmd.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return new ProductionStats
            {
                TotalBatchesCompleted = reader.GetInt32(0),
                TotalLitersProduced = reader.GetInt32(1),
                TotalAcceptableProducts = reader.GetInt32(2),
                TotalDefectProducts = reader.GetInt32(3)
            };
        }
        
        return new ProductionStats();
    }

    private async Task UpdateProductionSummaryAsync()
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = conn.CreateCommand();
        
        cmd.CommandText = @"
            UPDATE production_summary 
            SET 
                total_batches_completed = (SELECT COUNT(*) FROM batch_history WHERE status = 'Completed'),
                total_liters_produced = (SELECT COALESCE(SUM(amount_liters), 0) FROM batch_history WHERE status = 'Completed'),
                total_acceptable_products = (SELECT COALESCE(SUM(acceptable_count), 0) FROM batch_history WHERE status = 'Completed'),
                total_defect_products = (SELECT COALESCE(SUM(defect_count), 0) FROM batch_history WHERE status = 'Completed'),
                last_updated = CURRENT_TIMESTAMP
            WHERE id = 1";
        
        await cmd.ExecuteNonQueryAsync();
    }
}

// Model for production statistics
public class ProductionStats
{
    public int TotalBatchesCompleted { get; set; }
    public int TotalLitersProduced { get; set; }
    public int TotalAcceptableProducts { get; set; }
    public int TotalDefectProducts { get; set; }
    
    public double DefectRate => TotalLitersProduced > 0 
        ? (TotalDefectProducts / (double)(TotalAcceptableProducts + TotalDefectProducts)) * 100 
        : 0;
}