using System.Collections.Generic;
using System.Linq;


public class BatchQueue
{
    private static BatchQueue _instance = new BatchQueue();

    public int? Priority { get; set; }

    private readonly object _lock = new();

    private Batch _batch;

    // Priority queue (higher number => higher priority)
    // .NET PriorityQueue dequeues the *smallest* priority first,
    // so we store -priority to make larger numbers come out first.
    private PriorityQueue<Batch, int> _batchQueue = new();

    //
    public static BatchQueue Instance => _instance;

    public enum BatchPriority
    {
        High = 2,
        Medium = 1,
        Low = 0
    }

    //Enqueue is add a batch to the queue
    public void EnqueueBatch(Batch batch, BatchPriority priority = BatchPriority.Low)
    {
        lock (_lock)
        {
            switch (priority)
            {
                case BatchPriority.Low:
                    _batchQueue.Enqueue(batch, 0);
                    break;
                case BatchPriority.Medium:
                    _batchQueue.Enqueue(batch, 1);
                    break;
                case BatchPriority.High:
                    _batchQueue.Enqueue(batch, 2);
                    break;
            }
        }
    }

    //
    public Batch DequeueBatch()
    {
        lock (_lock)
        {
            return _batchQueue.Dequeue();
        }
    }

    public PriorityQueue<Batch, int> GetAllBatches()
    {
        lock (_lock)
        {
            return _batchQueue;
        }
    }

    //Remove batch by id
    public void RemoveBatch(int id)
    {
        //Temporarily convert queue to list to remove specific batch
        List<(string, int)> tempList = new List<(string, int)>();
        lock (_lock)
        {
            while (_batchQueue.Count > 0)
            {
                var priority = _batchQueue.Peek();
                var item = _batchQueue.Dequeue();
                if (item.Id != id)
                {
                    tempList.Add((item.Id, priority));
                }
            }
        }

        var batchToRemove = tempList.FirstOrDefault(b => b.Id == id);
        if (batchToRemove != null)
        {
            tempList.Remove(batchToRemove);
            //Rebuild the queue after removing the specific batch
            _batchQueue = new PriorityQueue<Batch, int>(tempList);
        }
    }

    // Enqueue (with priority):
}