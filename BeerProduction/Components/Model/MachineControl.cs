using Opc.UaFx.Client;

public class MachineControl
{
    public string MachineURL { get; }

    public int MachineID { get; }
    
    public string MachineName { get; }

    public OpcClient Client { get; }
    
    public bool IsConnected => Client?.State == OpcClientState.Connected;
    
    /// <summary>
    /// Constructor for a machine
    /// </summary>
    public MachineControl(int machineId, string machineURL, string machineName)
    {
        MachineID = machineId;
        MachineURL = machineURL;
        MachineName = machineName;
        Client = new OpcClient(machineURL);
        // Optional: subscribe to the Connected event
        Client.Connected += ( _ , _ ) =>
        {
            Console.WriteLine($"✅ Machine {MachineID} connected.");
        };
    }

    /// <summary>
    /// Tries to connect to machine, retrying up to maxTries times with delay between attempts
    /// </summary>
    public async Task TryConnectAsync(int maxTries = 2, int delay = 50)
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

    /// <summary>
    /// Disconnects from the machine
    /// </summary>
    public void Disconnect()
    {
        if (Client?.State == OpcClientState.Connected)
        {
            Client.Disconnect();
            Console.WriteLine($"🔌 Machine {MachineID} disconnected.");
        }
    }
}