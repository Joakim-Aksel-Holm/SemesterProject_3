using Opc.UaFx.Client;

public class MachineControl
{
    public string MachineURL { get; }

    public int MachineID { get; }
    
    public string MachineName { get; }

    public OpcClient Client { get; }
    
    public bool IsConnected => Client?.State == OpcClientState.Connected;
    
    //Constructor 
    public MachineControl(int machineId, string machineURL, string machineName)
    {
        MachineID = machineId;
        MachineURL = machineURL;
        MachineName = machineName;
        Client = new OpcClient(machineURL);
        // Optional: subscribe to the Connected event
        Client.Connected += (s, e) =>
        {
            Console.WriteLine($"✅ Machine {MachineID} connected.");
        };
    }

    public async Task TryConnectAsync(int maxTries = 1, int delay = 50)
    {
        if (IsConnected) return;

        for (int attempt = 1; attempt <= maxTries; attempt++)
        {
            try
            {
                Console.WriteLine($"Attempt {attempt}/{maxTries} connecting to {MachineName} ({MachineURL})");
                
                await Task.Run(() => Client.Connect());
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Attempt {attempt} failed for {MachineName} connection failed: {ex.Message}");
            
                if (attempt < maxTries) await Task.Delay(delay);
            }
        }
        Console.WriteLine($"Failed to connect to {MachineName} after {maxTries} attempts");
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