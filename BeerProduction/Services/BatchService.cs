/*using BeerProduction.Components.Model;
using BeerProduction.Enums;

namespace BeerProduction.Services;

public class BatchService
{
    private BatchQueue _batches;

    public BatchService()
    {
        _batches = BatchQueue.Instance;
    }

    public void AddBatch(Batch batch, BatchPriority priority = BatchPriority.Low)
    {
        _batches.EnqueueBatch(batch, priority);
    }

    //Remove a specific batch by ID (removes the first match)
    public bool RemoveBatch(int id)
    {
        return _batches.RemoveBatch(id);
    }

    //Get all batches ordered by priority (highest first)
    public List<Batch> GetBatches()
    {
        return _batches.ToOrderedListHighestFirst();
    }


    public int GetBatchCount()
    {
        return _batches.GetAllBatches().Count;
    }
}*/