using BeerProduction.Services;

namespace BeerProduction.Tests;

public class BatchQueueTests
{
    [Fact]
    public void TestEnqueueDequeueRemove()
    {
        var queue = BatchQueue.Instance;
        
        //Clear the queue for test purpose only, so that we start with a empty queue.
        while (queue.DequeueBatch() != null){}
        
        var batchLow = new Batch(1, beerType: 0, quantity: 100, speed: 10, expiryDate: DateTime.Now.AddDays(365));
        var batchHigh = new Batch(2, beerType: 1, quantity: 50, speed: 20, expiryDate: DateTime.Now.AddDays(365));
        var batchMedium = new Batch(3, beerType: 2, quantity: 75, speed: 15, expiryDate: DateTime.Now.AddDays(365));

        // Enqueue batches with priorities
        queue.EnqueueBatch(batchLow, BatchQueue.BatchPriority.Low);
        queue.EnqueueBatch(batchHigh, BatchQueue.BatchPriority.High);
        queue.EnqueueBatch(batchMedium, BatchQueue.BatchPriority.Medium);
        
        // Check order (highest-priority first)
        var ordered = queue.ToOrderedListHighestFirst().Select(b => b.Id).ToList();
        Assert.Equal(new[] { 2, 3, 1 }, ordered);

        // Remove batch with Id=2 (High priority)
        bool removed = queue.RemoveBatch(2);
        Assert.True(removed);

        // Check order after removal
        var orderedAfter = queue.ToOrderedListHighestFirst().Select(b => b.Id).ToList();
        Assert.Equal(new[] { 3, 1 }, orderedAfter);

        // Dequeue next batch (should be Id=3, Medium)
        var dequeued = queue.DequeueBatch();
        Assert.Equal(3, dequeued.Id);

        // Dequeue remaining batch (should be Id=1, Low)
        dequeued = queue.DequeueBatch();
        Assert.Equal(1, dequeued.Id);

        // Queue should now be empty
        Assert.Null(queue.DequeueBatch());
        Assert.Equal(0, queue.Count);
    }
    
    public void TestAddBatch()
    {
        var service = new BatchService();
        var queue = BatchQueue.Instance;
        
        
        while (queue.DequeueBatch() != null){}
        
        Assert.Equal(0, service.GetBatchCount());

        var batch = new Batch(1,0,100,10,DateTime.Now.AddDays(365));
        service.AddBatch(batch, BatchQueue.BatchPriority.High);
        
        Assert.Equal(1, service.GetBatchCount());
        
        
        var batches = service.GetBatches();
        Assert.Contains(batches, b=> b.Id == 1);
        
        var orderdIds = service.GetBatches().Select(b=>b.Id).ToList();
        Assert.Equal(new[] {1}, orderdIds);
    }
}
