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
        TryConnect();
        // Optional: subscribe to the Connected event
        Client.Connected += (s, e) =>
        {
            Console.WriteLine($"✅ Machine {MachineID} connected.");
        };
        
    }

    public bool TryConnect()
    {
        try
        {
            if (Client.State != OpcClientState.Connected)
            {
                Console.WriteLine($"🔌 Attempting connection to {MachineURL} ...");
                Client.Connect();
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Machine {MachineID} connection failed: {ex.Message}");
            return false;
        }
    }

    public void Disconnect()
    {
        if (Client?.State == OpcClientState.Connected)
        {
            Client.Disconnect();
            Console.WriteLine($"🔌 Machine {MachineID} disconnected.");
        }
    }
}