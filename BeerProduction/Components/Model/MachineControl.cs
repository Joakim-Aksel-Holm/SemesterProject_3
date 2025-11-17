using System.Reflection.PortableExecutable;
using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;

public class MachineControl
{
    public string MachineURL { get; }

    public int MachineID { get; }
    
    public string MachineName { get; }

    public OpcClient Client { get; set; }
    
    public bool IsConnected => Client?.State == OpcClientState.Connected;


    //Constructor 
    public MachineControl(int machineId, string machineURL, string machineName)
    {
        MachineID = machineId;
        MachineURL = machineURL;
        MachineName = machineName;
        Client = new OpcClient(machineURL);
        Client.Connect();
    }
}