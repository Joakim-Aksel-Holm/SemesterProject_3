using System.ComponentModel.DataAnnotations;
using BeerProduction.Enums;

namespace BeerProduction.Components.Model;

public class BatchForm
{
    /// <summary>
    /// Attributes for batch form
    /// </summary>
    [Required(ErrorMessage = "Amount is required")]
    public int Size { get; set; }

    [Required(ErrorMessage = "Speed is required")]
    public float Speed { get; set; }
    
    [Required(ErrorMessage = "Must select beer type")]
    public BeerType SelectedBeerType { get; set; } 


}