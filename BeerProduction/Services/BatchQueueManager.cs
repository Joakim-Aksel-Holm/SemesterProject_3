using BeerProduction.Components.Model;
using BeerProduction.Enums;

namespace BeerProduction.Services;

public class BatchQueue
{
    private static readonly BatchQueue _instance = new BatchQueue();
    public static BatchQueue Instance => _instance;

    private readonly object _lock = new();

    // Priority queue (higher number => higher priority)
    // .NET PriorityQueue dequeues the smallest priority first,
    // so we use numeric priorities as-is and reverse in ToOrderedListHighestFirst.
    public static PriorityQueue<Batch, int> _batchQueue = new();

    
    // =========================================================================
    // Methods for the batch queue
    // =========================================================================
    
    /// <summary>
    /// Enqueue a batch with optional priority 
    /// </summary>
    public void EnqueueBatch(Batch batch, BatchPriority priority = BatchPriority.Low)
    {
        lock (_lock)
        {
            _batchQueue.Enqueue(batch, -(int)priority);
        }
    }
    
    /// <summary>
    /// Dequeue the next batch (returns null if empty)
    /// </summary>
    public Batch DequeueBatch()
    {
        lock (_lock)
        {
            if (_batchQueue.TryDequeue(out var batch, out _))
                return batch;
            return null;
        }
    }
    
    /// <summary>
    /// Return all batches in unordered form (heap order)
    /// </summary>
    public List<(Batch Batch, int Priority)> GetAllBatches()
    {
        lock (_lock)
        {
            return _batchQueue.UnorderedItems
                .Select(t => (t.Element, t.Priority))
                .ToList();
        }
    }
    
    /// <summary>
    /// Return ordered list of batches (highest priority first)
    /// </summary>
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
    
    /// <summary>
    /// Remove a specific batch by ID
    /// </summary>
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

    /// <summary>
    /// Returns the list of batches ordered by ID
    /// </summary>
    public List<Batch> ToOrderedListIDFirst()
    {
        lock (_lock)
        {
            var drained = new List<(Batch batch, int ID)>();

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
                .OrderBy(t => t.ID)
                .Select(t => t.batch)
                .ToList();
        }
    }

    /// <summary>
    /// Prints the batch queue
    /// </summary>
    public void BatchQueuePrint()
    {
        lock (_lock)
        {
            foreach (var (batch, priority) in _batchQueue.UnorderedItems)
            {
                Console.WriteLine($"Batch ID: {batch.Id}, Priority: {priority}");
                Console.WriteLine("Beer type " + batch.BeerType);
                Console.WriteLine("Amount of beers " + batch.Size);
                Console.WriteLine("Machine speed " + batch.Speed);
                Console.WriteLine();
            }
        }
    }

    /// <summary>
    /// Returns the queue
    /// </summary>
    public static PriorityQueue<Batch, int> GetQueue()
    {
        return _batchQueue;
    }
}