using BeerProduction.Components.Model;
using BeerProduction.Enums;
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
        
        var batchLow = new Batch(1, beerType: BeerType.Pilsner, size: BatchQuantity.Small, speed: MachineSpeed.PilsnerSlow);
        var batchHigh = new Batch(2, beerType: BeerType.Wheat, size: BatchQuantity.Medium, speed: MachineSpeed.WheatMedium);
        var batchMedium = new Batch(3, beerType: BeerType.IPA, size: BatchQuantity.Large, speed: MachineSpeed.IPAMedium);

        // Enqueue batches with priorities
        queue.EnqueueBatch(batchLow, BatchPriority.Low);
        queue.EnqueueBatch(batchHigh, BatchPriority.High);
        queue.EnqueueBatch(batchMedium, BatchPriority.Medium);
        
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

    [Fact]
    public void TestAddBatch()
    {
        var service = new BatchService();
        var queue = BatchQueue.Instance;
        
        
        while (queue.DequeueBatch() != null){}
        
        Assert.Equal(0, service.GetBatchCount());

        var batch = new Batch(1,0,BatchQuantity.Medium,MachineSpeed.AlcoholFreeFast,BatchPriority.Medium);
        service.AddBatch(batch, BatchPriority.High);
        
        Assert.Equal(1, service.GetBatchCount());
        
        
        var batches = service.GetBatches();
        Assert.Contains(batches, b=> b.Id == 1);
        
        var orderdIds = service.GetBatches().Select(b=>b.Id).ToList();
        Assert.Equal(new[] {1}, orderdIds);
    }
}
