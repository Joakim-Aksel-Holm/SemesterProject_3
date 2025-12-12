using System.ComponentModel.DataAnnotations;
using BeerProduction.Enums;

namespace BeerProduction.Components.Model;

public class BatchForm
{
    /// <summary>
    /// Attributes for batch form
    /// </summary>
    [Required(ErrorMessage = "Amount is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public int Size { get; set; }

    [Required(ErrorMessage = "Speed is required")]
    [Range(1, 600, ErrorMessage = "Amount must be between 1 - 600")]
    public float Speed { get; set; }

    [Required(ErrorMessage = "Must select beer type")]
    public BeerType SelectedBeerType { get; set; }
}