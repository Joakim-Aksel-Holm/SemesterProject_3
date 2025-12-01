using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;
using BeerProduction.Enums;

namespace BeerProduction.Components.Model;

public class Batch
{
    public int Id { get; }
    [Required]
    public BeerType BeerType { get; set; }
    public int Size { get; set; }
    public float Speed { get; set; }
    public BatchPriority Priority { get; set; }
    public MachineState CurrentState { get; set; }
    public DateTime ManufactureDate { get; }
    
    public string? Notes { get; set; }
    
    // Navigation property:
    public Machine? Machine { get; set; }


    /// <summary>
    /// Constructore for a batch
    /// </summary>
    // Priority default is low
    public Batch(int id, BeerType beerType, int size, float speed,
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