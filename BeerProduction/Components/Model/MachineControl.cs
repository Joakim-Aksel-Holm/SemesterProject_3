using System.Reflection.PortableExecutable;
using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;

public class MachineControl
{
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