namespace BeerProduction.Services;

public class BatchQueue
{
    private static readonly BatchQueue _instance = new BatchQueue();
    public static BatchQueue Instance => _instance;

    private readonly object _lock = new();

    // Priority queue (higher number => higher priority)
    // .NET PriorityQueue dequeues the smallest priority first,
    // so we use numeric priorities as-is and reverse in ToOrderedListHighestFirst.
    private PriorityQueue<Batch, int> _batchQueue = new();

    public enum BatchPriority
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    // --------------------------------------------
    // Enqueue a batch with optional priority
    // --------------------------------------------
    public void EnqueueBatch(Batch batch, BatchPriority priority = BatchPriority.Low)
    {
        lock (_lock)
        {
            _batchQueue.Enqueue(batch, -(int)priority);
            
        }
    }

    // --------------------------------------------
    // Dequeue the next batch (returns null if empty)
    // --------------------------------------------
    public Batch DequeueBatch()
    {
        lock (_lock)
        {
            if (_batchQueue.TryDequeue(out var batch, out _))
                return batch;
            return null;
        }
    }

    // --------------------------------------------
    // Return all batches in unordered form (heap order)
    // --------------------------------------------
    public List<(Batch Batch, int Priority)> GetAllBatches()
    {
        lock (_lock)
        {
            return _batchQueue.UnorderedItems
                .Select(t => (t.Element, t.Priority))
                .ToList();
        }
    }

    // --------------------------------------------
    // Return ordered list of batches (highest priority first)
    // --------------------------------------------
    public List<Batch> ToOrderedListHighestFirst()
    {
        lock (_lock)
        {
            var drained = new List<(Batch batch, int priority)>();

            // Drain queue (min-priority first)
            while (_batchQueue.TryDequeue(out var batch, out var priority))
            {
                drained.Add((batch, priority));
            }

            // Rebuild the queue
            _batchQueue = new PriorityQueue<Batch, int>();
            foreach (var (b, p) in drained)
                _batchQueue.Enqueue(b, p);

            // Return list highest-priority first
            return drained
                .OrderBy(t => t.priority)
                .Select(t => t.batch)
                .ToList();
        }
    }

    // --------------------------------------------
    // Remove a specific batch by ID
    // --------------------------------------------
    public bool RemoveBatch(int id)
    {
        lock (_lock)
        {
            var temp = new List<(Batch batch, int priority)>();
            bool removed = false;

            // Drain queue and skip the batch with the matching id
            while (_batchQueue.TryDequeue(out var batch, out var priority))
            {
                if (!removed && batch.Id == id)
                {
                    removed = true;
                    continue;
                }

                temp.Add((batch, priority));
            }

            // Rebuild queue from remaining items
            _batchQueue = new PriorityQueue<Batch, int>();
            foreach (var (b, p) in temp)
                _batchQueue.Enqueue(b, p);

            return removed;
        }
    }

    // Optional: Count of batches
    public int Count
    {
        get
        {
            lock (_lock) return _batchQueue.Count;
        }
    }
}