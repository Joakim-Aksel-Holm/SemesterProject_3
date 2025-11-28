namespace BeerProduction.Services;
using System.Threading.Tasks;
using BeerProduction.Components.Model;
using BeerProduction.Enums;


public class MachineControlService
{
    public MachineControl MachineControl { get; }
    public BatchQueue BatchQueue {get;set;}

    public MachineControlService(MachineControl machineControl, BatchQueue batchQueue)
    {
        MachineControl = machineControl;
        BatchQueue = batchQueue;
    }
    public int TotalInProcutionMachines { get; set; } = 0;

    //todo list:
    //todo: Online machines method. "missing refining front-end . Call to front-end team"
    //todo: In Production (running) machines. "Done"
    //todo: Total Produced Batches method. "Still In progress"
    /// <summary>
    /// everything should work as planned, everything is done to perfection, the only issue might be cuz of the start method that we start out production with!
    /// perhaps we should wait until the start method is done on the other branch so that we take out the doubt about it, being a start-method issue!
    /// </summary>
    /// <returns></returns>
    //todo: Defect rate method.
    //todo: Method for  Produce Amount.
    //todo: Method for Temperature sensor reading.
    //todo: Method for Humidity sensor reading.
    //todo: Method for Vibration sensors reading.
    //todo: Method for defect products on each machine.
    //todo: Method for Accepted products on each machine.
    //todo: Method for Batch process progression rate.
    //todo: Method forMaintenance status progression rate.
    //todo: Method for Current Batch (ID).
    //todo: Method for Current Batch beer type.
    
public class MachineControlService(MachineControl machineControl)
{
    /// <summary>
    /// Initializes a new instance of the MachineControl class.
    /// </summary>
    public MachineControl MachineControl { get; } = machineControl;

    // =========================================================================
    // SIMPLE PROPERTY METHODS (Fast access - Can be sync or async)
    // =========================================================================

    // Reads the Batch ID value
    public int GetMachineId()
    {
        return MachineControl.MachineID;
    }

    /// <summary>
    /// Gets the machine name from local property (fast access)  
    /// </summary>
    public string GetMachineName() => machineControl.MachineName;
    
    public bool IsConnected() => machineControl.IsConnected;

    // =========================================================================
    // OPC SAFE READ METHODS (Safe reads - Can be sync or async)
    // =========================================================================
    
    /// <summary>
    /// Returns the value of the specified node if the connection is active, otherwise returns the fallback value
    /// </summary>
    private T? SafeRead<T>(string nodeId, T? fallback = default)
    {
        if (!MachineControl.IsConnected)
        {
            return fallback;
        }

        try
        {
            return MachineControl.Client.ReadNode(nodeId).As<T>();
        }
        catch
        {
            return fallback;
        }
    }
    
    // =========================================================================
    // OPC READ METHODS (Fast sensor reads - Keep SYNC)
    // =========================================================================

    /// <summary>
    /// Reads the current Batch ID from OPC server (fast read)
    /// </summary>
    public int GetBatchId()
    {
        return SafeRead("ns=6;s=::Program:Cube.Status.Parameter[0].Value", -1);
    }

    /// <summary>
    /// Reads the product amount from OPC server (fast read)
    /// </summary>
    public int GetAmount()
    {
        return SafeRead("ns=6;s=::Program:Cube.Status.Parameter[1].Value", -1);
    }

    /// <summary>
    /// Reads the products per minute from OPC server (fast read)
    /// </summary>
    public int GetPpm()
    {
        return SafeRead("ns=6;s=::Program:Cube.Status.MachSpeed", -1);
    }

    /// <summary>
    /// Reads the temperature sensor from OPC server (fast read)
    /// </summary>
    public float GetTemperature()
    {
        return SafeRead("ns=6;s=::Program:Cube.Status.Parameter[3].Value", -1f);
    }

    /// <summary>
    /// Reads the humidity sensor from OPC server (fast read)
    /// </summary>
    public decimal GetHumidity()
    {
        return SafeRead("ns=6;s=::Program:Cube.Status.Parameter[2].Value", -1m);
    }

    /// <summary>
    /// Reads the vibration sensor from OPC server (fast read)
    /// </summary>
    public decimal GetVibration()
    {
        return SafeRead("ns=6;s=::Program:Cube.Status.Parameter[4].Value", -1m);
    }

    /// <summary>
    /// Reads the defective products count from OPC server (fast read)
    /// </summary>
    public int GetDefects()
    {
        return SafeRead("ns=6;s=::Program:Cube.Admin.ProdDefectiveCount", -1);
    }

    /// <summary>
    /// Reads the total produced products count from OPC server (fast read)
    /// </summary>
    public int GetProduced()
    {
        return SafeRead("ns=6;s=::Program:Cube.Admin.ProdProcessedCount", -1);
    }

    /// <summary>
    /// Reads the current machine status from OPC server (fast read)
    /// </summary>
    public int GetStatus()
    {
        return SafeRead("ns=6;s=::Program:Cube.Status.StateCurrent.Value", -1);
    }

    /// <summary>
    /// Reads the current beer type from OPC server and converts to enum (fast read)
    /// </summary>
    public BeerType GetCurrentBatch()
    {
        var current = SafeRead("ns=6;s=::Program:Cube.Admin.ProdProcessedCount.Value", -1);
        return Enum.IsDefined(typeof(BeerType), current) ? (BeerType)current : BeerType.Pilsner;
    }

    /// <summary>
    /// Reads the maintenance counter from OPC server (fast read)
    /// </summary>
    public int GetMaintenanceStatus()
    {
        var counter = SafeRead("ns=6;s=::Program:Maintenance.Counter", 0);
        const int MAINTENANCE_CYCLE = 30000;
        
        var percentage = (counter % MAINTENANCE_CYCLE) / (double)MAINTENANCE_CYCLE * 100;
        return (int)Math.Round(percentage);
    } 

    // =========================================================================
    // CALCULATION METHODS (Good candidates for ASYNC in web contexts)
    // =========================================================================

    /// <summary>
    /// Calculates the defect rate percentage based on produced and defective counts
    /// </summary>
    public async Task<double> GetDefectRateAsync()
    {
        var total = GetProduced();   // Fast sync read
        var defects = GetDefects();  // Fast sync read
        return await Task.FromResult(total > 0 ? (defects / (double)total) * 100 : 0);
    }

    /// <summary>
    /// Calculates the number of acceptable products (produced - defects)
    /// </summary>
    public async Task<int> GetAcceptableAsync()
    {
        var produced = GetProduced(); // Fast sync read
        var defects = GetDefects();   // Fast sync read  
        return await Task.FromResult(produced - defects);
    }
    /// <summary>
    /// Calculates the number of acceptable products (sync)
    /// </summary>
    /// <returns></returns>
    public int GetAcceptable()
    {
        var produced = GetProduced();
        var defects = GetDefects();
        return SafeRead("ns=6;s=::Program:Product.good.Value" ,-1);
    }

    /// <summary>
    /// Calculates the batch completion percentage (produced / amount * 100)
    /// </summary>
    public async Task<int> GetBatchProcessAsync()
    {
        var produced = GetProduced(); // Fast sync read
        var amount = GetAmount();     // Fast sync read
        return await Task.FromResult(amount > 0 ? (int)((produced / (double)amount) * 100) : 0);    
    }

    /// <summary>
    /// Checks if the machine is online based on OPC connection status
    /// </summary>
    public async Task<string> GetOnlineAsync()
    {
        var serverStatus = MachineControl.IsConnected;
        return await Task.FromResult(serverStatus ? "Online" : "Offline");
    }

    // =========================================================================
    // MACHINE CONTROL METHODS (Write commands - Keep SYNC)
    // =========================================================================

    /// <summary>
    /// Sets the change request flag to true (triggers command processing)
    /// </summary>
    public void SetChangeRequestTrue()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CmdChangeRequest", true);
    }

    /// <summary>
    /// Sends reset command to the machine
    /// </summary>
    public void ResetCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 1);
        SetChangeRequestTrue();
    }

    /// <summary>
    /// Sends start command to the machine
    /// </summary>
    public void StartCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 2);
        SetChangeRequestTrue();
    }

    /// <summary>
    /// Sends stop command to the machine
    /// </summary>
    public void StopCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 3);
        SetChangeRequestTrue();
    }

    /// <summary>
    /// Sends abort command to the machine
    /// </summary>
    public void AbortCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 4);
        SetChangeRequestTrue();
    }

    /// <summary>
    /// Sends clear command to the machine
    /// </summary>
    public void ClearCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 5);
        SetChangeRequestTrue();
    }

    // =========================================================================
    // COMPLEX OPERATION METHODS (With delays - Make ASYNC)
    // =========================================================================

    /// <summary>
    /// Smart machine startup sequence with status checking and async delays
    /// </summary>
    public async Task StartMachineAsync()
    {
        int statusVal = GetStatus(); // Fast sync read

        // States where machine is already running or cannot start
        if (statusVal == 0 || statusVal == 3 || statusVal == 6 || statusVal == 11)
        {
            return;
        }
        // States requiring reset before start
        else if (statusVal == 2 || statusVal == 5 || statusVal == 17)
        {
            ResetCommand();
            await Task.Delay(1000); // Wait for reset to process
            StartCommand();
        }
        // State where machine can start directly
        else if (statusVal == 4)
        {
            StartCommand();
        }
        // State requiring clear and reset before start
        else if (statusVal == 9)
        {
            ClearCommand();
            await Task.Delay(1000); // Wait for clear to process
            ResetCommand();
            await Task.Delay(1000); // Wait for reset to process
            StartCommand();
        }
        // Unknown state - retry after delay
        else
        {
            await Task.Delay(2000);
            await StartMachineAsync(); // Recursive retry
        }
    }

    /// <summary>
    /// Stops the machine if it's not already in stopped state
    /// </summary>
    public async Task AutomatedStart()
    {
        while (BatchQueue.Count > 0)
        {
            Console.WriteLine("Starting batch queue");
            Batch nextBatch = BatchQueue.DequeueBatch();
            
            if (nextBatch == null)
                break; //Stop når der ikke er flere batches

          Console.WriteLine($"[ID: {nextBatch.Id}] - Priority: {nextBatch.Priority} | Type: {nextBatch.BeerType} | Amount: {nextBatch.Size}");

            await AddBatchAsync(nextBatch);

            //Start maskinen
            await StartMachineAsync();


            while (true)
            {
                int state = GetStatus();
                if (state == 17) //17 is completed
                    break;
                await Task.Delay(500);
            }

            Console.WriteLine($"Batch {nextBatch.Id} completed.");
        }

        Console.WriteLine("All batches processed. Machine queue is empty.");
    }


    public async Task StopMachineAsync()
    {
        int statusVal = GetStatus();

        if (statusVal == 9)
            return;

        await StopCommandAsync();
    }
    public async Task AbortMachineAsync()
    {
        Console.WriteLine("Aborting machine...");
        await AbortCommandAsync();
    }

    public async Task ClearMachineAsync()
    {
        Console.WriteLine("Clearing machine...");
        await ClearCommandAsync();
    }

    public async Task ResetMachineAsync()
    {
        Console.WriteLine("Resetting machine...");
        await ResetCommandAsync();
    }

    public async Task AddBatchAsync(Batch _batch)
    {
        int BatchId = _batch.Id;
        BeerType BeerType = _batch.BeerType;
        int Size = _batch.Size;
        float Speed = _batch.Speed;


        await Task.Run(() =>
            MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.Parameter[0].Value", (float)BatchId));
        await Task.Run(() =>
            MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.Parameter[1].Value", (float)BeerType));
        await Task.Run(() =>
            MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.Parameter[2].Value", (float)Size));
        await Task.Run(() => MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.MachSpeed", (float)Speed));
    }

    // public async Task QueueBatchMachineAsync(PriorityQueue<Batch, int> _batch)
    // {
    //     await AddBatchAsync(_batch.Peek());
    //     _batch.Dequeue();
    // }
}
