using System.ComponentModel.DataAnnotations;
using BeerProduction.Components.Enums;

namespace BeerProduction.Components.Model
{
    public class BatchForm
    {
        [Required(ErrorMessage = "BeerType is required")]
        public BeerType BeerType { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public BatchQuantitySize Size { get; set; }

        [Required(ErrorMessage = "Speed is required")]
        public MachineSpeed Speed { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public BatchPriority Priority { get; set; }
    }
}
