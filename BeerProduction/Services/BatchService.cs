using System.Collections.Generic;
using System.Linq;

public class BatchService
{
    private BatchQueue _batches;

    public BatchService()
    {
        _batches = BatchQueue.Instance;
    }

    public void AddBatch(Batch batch)
    {
        _batches.EnqueueBatch(batch);
    }

    public void RemoveBatch(int id)
    {
        _batches.RemoveBatch(id);
    }

    public Queue<Batch> GetBatches()
    {
        return _batches.GetAllBatches();
    }

    public int GetBatchCount()
    {
        return _batches.GetAllBatches().Count;
    }
}
