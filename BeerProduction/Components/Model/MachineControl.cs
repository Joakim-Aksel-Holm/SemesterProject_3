using System.Reflection.PortableExecutable;
using Opc.UaFx;
using Opc.UaFx.Client;

public class MachineControl
{
    public float Tempurature { get; }
    public int BatchId { get; }
    public int Speed { get; }
    public bool AcceptedProduct { get; }
    public int QuantityAcceptedProduct { get; }

    public int status { get; }

    public string MachineURL { get; }

    public int MachineID { get; }


    //Constructor 
    public MachineControl(int machineId, string machineURL)
    {
        MachineID = machineId;
        MachineURL = machineURL;
        var client = new OpcClient(machineURL);
    }
     
 }