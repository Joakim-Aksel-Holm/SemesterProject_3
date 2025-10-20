using System.Collections.Generic;
using System.Linq;

public class BatchService
{
    private readonly List<Batch> _batches = new();

    public void AddBatch(Batch batch)
    {
        _batches.Add(batch);
    }

    public void RemoveBatch(int id)
    {
        var batch = _batches.FirstOrDefault(b => b.Id == id);
        _batches.Remove(batch);
    }

    public List<Batch> GetBatches()
    {
        return _batches;
    }
}
