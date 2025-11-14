namespace BeerProduction.Components.Model;

public class Machine
{
    public int MachineId { get; set; }
    public string MachineUrl { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Operational";
    public DateTime LastMaintenance { get; set; } = DateTime.Now;
    public string Location { get; set; } = string.Empty;


    // for the machind card:
    public decimal Temperature { get; set; }
    public decimal Pressure { get; set; }
    public int BatchProgress { get; set; } // 0-100%
    public int ProductionRate { get; set; } // units/minute
    public int DefectCount { get; set; }
    public int AcceptableCount { get; set; }
    public string CurrentBatch { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
}