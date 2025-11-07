using System.Reflection.PortableExecutable;
using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;

public class MachineControl
{
    public int BatchId { get; }
    public int Amount { get; }
    public int PPM { get; }
    public float Temperature { get; }
    public decimal Humidity { get; }
    public decimal Vibration { get; }
    public int Defects { get; }
    public int Acceptable { get; }

    public int MaintenanceStatus { get; }

    public string MachineURL { get; }

    public int MachineID { get; }

    public OpcClient Client { get; set; }


    //Constructor 
    public MachineControl(int machineId, string machineURL)
    {
        MachineID = machineId;
        MachineURL = machineURL;
        Client = new OpcClient(machineURL);
        Client.Connect();
    }

}