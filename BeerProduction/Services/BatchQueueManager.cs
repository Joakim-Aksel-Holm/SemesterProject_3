using System.Collections.Generic;
using System.Linq;

public class BatchQueue
{
    private static BatchQueue _instance = new BatchQueue();

    private Queue<Batch> _batchQueue = new Queue<Batch>();

    //
    public static BatchQueue Instance => _instance;

    //Enqueue is add a batch to the queue
    public void EnqueueBatch(Batch batch)
    {
        _batchQueue.Enqueue(batch);
    }
    //
    public Batch DequeueBatch()
    {
        return _batchQueue.Dequeue();
    }

    public Queue<Batch> GetAllBatches()
    {
        return _batchQueue;
    }
    //Remove batch by id
    public void RemoveBatch(int id)
    {
        //Temporarily convert queue to list to remove specific batch
        var tempList = _batchQueue.ToList();
        var batchToRemove = tempList.FirstOrDefault(b => b.Id == id);
        if (batchToRemove != null)
        {
            tempList.Remove(batchToRemove);
            //Rebuild the queue after removing the specific batch
            _batchQueue = new Queue<Batch>(tempList);
        }
    }

}