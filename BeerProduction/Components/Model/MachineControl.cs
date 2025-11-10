using Opc.UaFx;
using Opc.UaFx.Client;

public class MachineControl
{
    public float Temperature { get; private set; }
    public int BatchId { get; private set; }
    public int Speed { get; private set; }
    public bool AcceptedProduct { get; private set; }
    public int QuantityAcceptedProduct { get; private set; }
    public int Status { get; private set; }

    public string MachineURL { get; }
    public int MachineID { get; }
    public OpcClient Client { get; private set; }

    public bool IsConnected => Client?.State == OpcClientState.Connected;

    // Constructor (no auto-connect)
    public MachineControl(int machineId, string machineURL)
    {
        MachineID = machineId;
        MachineURL = machineURL;
        Client = new OpcClient(machineURL);

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
