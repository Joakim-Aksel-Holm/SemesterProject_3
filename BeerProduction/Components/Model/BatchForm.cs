using System.ComponentModel.DataAnnotations;
using BeerProduction.Enums;

namespace BeerProduction.Components.Model;

public class BatchForm
{


    [Required(ErrorMessage = "Amount is required")]
    public int Size { get; set; }

    [Required(ErrorMessage = "Speed is required")]
    public float Speed { get; set; }


}