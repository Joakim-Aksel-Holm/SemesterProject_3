namespace BeerProduction.Components.Model;

public class Machine
{
    public int MachineId { get; set; }
    public string MachineUrl { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Operational";
    public DateTime LastMaintenance { get; set; } = DateTime.Now;
    public string Location { get; set; } = string.Empty;
}