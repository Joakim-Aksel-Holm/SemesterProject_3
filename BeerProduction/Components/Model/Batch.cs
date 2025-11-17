using BeerProduction.Enums;

namespace BeerProduction.Components.Model;

public class Batch
{
    public int Id { get; }
    public BeerType BeerType { get; set; }
    public int Size { get; set; }
    public float Speed { get; set; }
    public BatchPriority Priority { get; set; }
    public MachineState CurrentState { get; set; }
    public DateTime ManufactureDate { get; }


    // Fixed constructor
    public Batch(int id, BeerType beerType, int size, int speed,
        BatchPriority priority = BatchPriority.Low)
    {
        Id = id;
        BeerType = beerType;
        Size = size;
        Speed = speed;
        Priority = priority;
        ManufactureDate = DateTime.Now;
        CurrentState = MachineState.Idle;
    }

}