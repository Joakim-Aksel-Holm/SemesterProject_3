using BeerProduction.Components.Model;
using BeerProduction.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public List<(Batch Batch, BatchPriorityKey Priority)> GetAllBatches()
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
                var drained = new List<(Batch batch, BatchPriorityKey priority)>();

                // Drain queue
                while (_batchQueue.TryDequeue(out var batch, out var priority))
                {
                    drained.Add((batch, priority));
                }

                // Rebuild queue with same comparer
                _batchQueue = new PriorityQueue<Batch, BatchPriorityKey>(new BatchPriorityComparer());
                foreach (var (b, p) in drained)
                    _batchQueue.Enqueue(b, p);

                // Sort: priority descending (highest first), then ID ascending
                return drained
                    .OrderByDescending(t => t.priority.Priority)
                    .ThenBy(t => t.priority.Id)
                    .Select(t => t.batch)
                    .ToList();
            }
        }

        // Return a new PriorityQueue ordered highest-first (note: PriorityQueue itself uses comparer)
        public PriorityQueue<Batch, BatchPriorityKey> ToOrderedQueueHighestFirst()
        {
            lock (_lock)
            {
                var items = _batchQueue.UnorderedItems
                    .Select(x => (batch: x.Element, priority: x.Priority))
                    .OrderByDescending(x => x.priority.Priority)
                    .ThenBy(x => x.priority.Id)
                    .ToList();

                var result = new PriorityQueue<Batch, BatchPriorityKey>(new BatchPriorityComparer());

                foreach (var item in items)
                {
                    result.Enqueue(item.batch, item.priority);
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
                var temp = new List<(Batch batch, BatchPriorityKey priority)>();
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
                _batchQueue = new PriorityQueue<Batch, BatchPriorityKey>(new BatchPriorityComparer());
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
                var drained = new List<(Batch batch, BatchPriorityKey Key)>();

                // Drain queue
                while (_batchQueue.TryDequeue(out var batch, out var key))
                {
                    drained.Add((batch, key));
                }

                // Rebuild the queue
                _batchQueue = new PriorityQueue<Batch, BatchPriorityKey>(new BatchPriorityComparer());
                foreach (var (b, p) in drained)
                    _batchQueue.Enqueue(b, p);

                // Return list ordered by ID ascending
                return drained
                    .OrderBy(t => t.Key.Id)
                    .Select(t => t.batch)
                    .ToList();
            }
        }

        public void BatchQueuePrint()
        {
            lock (_lock)
            {
                foreach (var (batch, priority) in _batchQueue.UnorderedItems)
                {
                    Console.WriteLine($"Batch ID: {batch.Id}, Priority: {priority.Priority}");
                    Console.WriteLine("Beer type " + batch.BeerType);
                    Console.WriteLine("Amount of beers " + batch.Size);
                    Console.WriteLine("Machine speed " + batch.Speed);
                    Console.WriteLine();
                }
            }
        }

        public PriorityQueue<Batch, BatchPriorityKey> GetQueue()
        {
            return _batchQueue;
        }
    }
}
