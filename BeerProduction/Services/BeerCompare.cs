using BeerProduction.Enums;
using BeerProduction.Services; // Ensure this is correct based on BatchPriorityKey definition


public class BatchPriorityComparer : IComparer<BatchPriorityKey>
{
    public int Compare(BatchPriorityKey x, BatchPriorityKey y)
    {
        int priorityComparison = y.Priority.CompareTo(x.Priority);

        if (priorityComparison != 0)
        {
            return priorityComparison;
        }

        return x.Id.CompareTo(y.Id);
    }
}