/*using BeerProduction.Components.Model;
namespace BeerProduction.Services;

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
*/