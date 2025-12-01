using BeerProduction.Components.Model;
using BeerProduction.Enums;

namespace BeerProduction.Services
{
    // Top-level key - Used for Comparer
    public readonly record struct BatchPriorityKey(int Priority, int Id);
    public class BatchPriorityComparer : IComparer<BatchPriorityKey>
    {
        public int Compare(BatchPriorityKey x, BatchPriorityKey y)
        {
            // Higher priority first
            int cmp = y.Priority.CompareTo(x.Priority);
            if (cmp != 0) return cmp;

            // If equal priority, lowest ID first
            return x.Id.CompareTo(y.Id);
            
        }
    }

    public class BatchQueue
    {
        private readonly PriorityQueue<Batch, BatchPriorityKey> _batchQueue;
        private readonly object _lock = new();

        public BatchQueue()
        {
            _batchQueue = new PriorityQueue<Batch, BatchPriorityKey>(new BatchPriorityComparer());
        }

        // --------------------------------------------
        // Enqueue a batch with optional priority
        // --------------------------------------------
        public void EnqueueBatch(Batch batch, BatchPriority priority = BatchPriority.Low)
        {
            lock (_lock)
            {
                
                var key = new BatchPriorityKey((int)priority, batch.Id);
                _batchQueue.Enqueue(batch, key);
                BatchAddedPrint(batch);
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



        public int Count
        {
            get
            {
                lock (_lock) return _batchQueue.Count;
            }
        }

        public void BatchQueuePrint()
        {
            lock (_lock)
            {
                Console.WriteLine("\n    Current Queue Status (Sorted)    ");

                var sortedBatches = _batchQueue.UnorderedItems
                    .OrderByDescending(item => item.Priority.Priority) 
                    .ThenBy(item => item.Priority.Id)                 
                    .Select(item => item.Element); 

                // Itterate through the batch queue
                if (!sortedBatches.Any())
                {
                    Console.WriteLine("Queue is empty.");
                }
                else
                {
                    foreach (var batch in sortedBatches)
                    {
                        Console.WriteLine($"[ID: {batch.Id}] - Priority: {batch.Priority} | Type: {batch.BeerType} | Amount: {batch.Size}");
                    }
                }
                Console.WriteLine("");
            }
        }


        public void BatchAddedPrint(Batch batch)
        {
            lock (_lock)
            {
                Console.WriteLine("\n     New Batch Added     ");
                Console.WriteLine($"[ID: {batch.Id}] - Priority: {batch.Priority} | Type: {batch.BeerType} | Amount: {batch.Size}");
                Console.WriteLine("\n");
            }
        }
    }
}
