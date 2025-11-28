    using BeerProduction.Components.Model;
    using BeerProduction.Enums;
    using System.Collections.Generic;

    namespace BeerProduction.Services;

    public class BatchQueue
    {
// Use BatchPriorityKey as the priority key type
    public readonly PriorityQueue<Batch, BatchPriorityKey> _batchQueue;

    // Define the record struct for the compound key
    public readonly record struct BatchPriorityKey(int Priority, int Id);

    public BatchQueue()
    {
      
    // Explicitly specify the generic types in the constructor call
    _batchQueue = new PriorityQueue<Batch, BatchPriorityKey>(new BatchPriorityComparer());


        private readonly object _lock = new();

        // Priority queue (higher number => higher priority)
        // .NET PriorityQueue dequeues the smallest priority first,
        // so we use numeric priorities as-is and reverse in ToOrderedListHighestFirst.


        // --------------------------------------------
        // Enqueue a batch with optional priority
        // --------------------------------------------
        public void EnqueueBatch(Batch batch, BatchPriority priority = BatchPriority.Low)
        {
            lock (_lock)
            {
                _batchQueue.Enqueue(batch, (int)priority);
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

        // Drain queue
        while (_batchQueue.TryDequeue(out var batch, out var priority))
        {
            drained.Add((batch, priority));
        }

        // Rebuild queue
        _batchQueue = new PriorityQueue<Batch, int>();
        foreach (var (b, p) in drained)
            _batchQueue.Enqueue(b, p);

        // Sort: HIGHEST priority first (descending), then ID (ascending)
        return drained
            .OrderByDescending(t => t.priority)  // Changed from OrderBy
            .ThenBy(t => t.batch.Id)
            .Select(t => t.batch)
            .ToList();
    }
}
    public PriorityQueue<Batch, int> ToOrderedQueueHighestFirst()
    {
        lock (_lock)
        {
            var items = _batchQueue.UnorderedItems
                .Select(x => (x.Element, x.Priority))
                .ToList();

            var ordered = items
                .OrderByDescending(x => x.Priority)
                .ThenBy(x => x.Element.Id)
                .ToList();

            var result = new PriorityQueue<Batch, int>();

            foreach (var (b, priority) in ordered)
            {
                result.Enqueue(b, priority);
            }

            return result;
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

        public void BatchQueuePrint()
        {
            lock (_lock)
            {
                foreach (var (batch, _) in _batchQueue.UnorderedItems)
                {
                    Console.WriteLine($"Batch ID: {batch.Id}, Priority: {batch.Priority}");
                    Console.WriteLine("Beer type " + batch.BeerType);
                    Console.WriteLine("Amount of beers " + batch.Size);
                    Console.WriteLine("Machine speed " + batch.Speed);
                    Console.WriteLine();
                }
            }
        }

        public PriorityQueue<Batch, int> GetQueue()
        {
            return _batchQueue;
        }
    }