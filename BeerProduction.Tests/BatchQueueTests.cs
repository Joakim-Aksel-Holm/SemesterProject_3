using BeerProduction.Components.Model;
using BeerProduction.Enums;
using BeerProduction.Services;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace BeerProduction.Tests;

public class BatchQueueTests
{
    [Fact]
    public void TestEnqueueDequeueRemove()
    {
        var queue = new BatchQueue();
        
        var batchLow = new Batch(1, beerType: BeerType.Pilsner, size: 500, speed: 200);
        var batchMedium = new Batch(3, beerType: BeerType.IPA, size: 5000, speed: 100);
        var batchMediumTie = new Batch(4, beerType: BeerType.Stout, 1000, speed: 140);
        var batchHigh = new Batch(2, beerType: BeerType.Wheat, size: 1000, speed: 200);

        // Enqueue batches with priorities
        queue.EnqueueBatch(batchLow);
        queue.EnqueueBatch(batchMediumTie, BatchPriority.Medium);
        queue.EnqueueBatch(batchHigh, BatchPriority.High);
        queue.EnqueueBatch(batchMedium, BatchPriority.Medium);

        // Dequeue next batch (should be Id=2, High)
        var dequeued = queue.DequeueBatch();
        Assert.Equal(2, dequeued.Id);

        // Dequeue next batch (should be Id=3, Medium)
        dequeued = queue.DequeueBatch();
        Assert.Equal(3, dequeued.Id);
        
        // Dequeue next batch (should be Id=4, Medium)
        dequeued = queue.DequeueBatch();
        Assert.Equal(4, dequeued.Id);
        
        // Dequeue next batch (should be Id=1, Low)
        dequeued = queue.DequeueBatch();
        Assert.Equal(1, dequeued.Id);
        
        // Queue should now be empty
        Assert.Null(queue.DequeueBatch());
        Assert.Equal(0, queue.Count);
    }
    
    [Fact]
    public void TestAddBatch()
    {
        var queue = new BatchQueue();
        
        Assert.Equal(0, queue.Count);

        var batch = new Batch(1, BeerType.AlcoholFree,size: 1000, speed: 125, BatchPriority.Medium);
        queue.EnqueueBatch(batch, BatchPriority.High);
        
        Assert.Equal(1, queue.Count);
    }
}
