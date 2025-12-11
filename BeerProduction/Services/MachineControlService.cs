using BeerProduction.Components.Model;
using BeerProduction.Enums;

namespace BeerProduction.Services;

public class MachineControlService
{
    private const decimal MaxInventory = 35000;


    public MachineControl MachineControl { get; }


    public BatchQueue? BatchQueue { get; }

    public MachineControlService(MachineControl machineControl, BatchQueue batchQueue)
    {
        MachineControl = machineControl;
        BatchQueue = batchQueue;
    }


    public MachineControlService(MachineControl machineControl)
    {
        MachineControl = machineControl;
    }

    public int GetMachineId() => MachineControl.MachineID;
    public string GetMachineName() => MachineControl.MachineName;
    public bool IsConnected() => MachineControl.IsConnected;


    private T? SafeRead<T>(string nodeId, T? fallback = default)
    {
        if (!MachineControl.IsConnected) return fallback;

        try
        {
            return MachineControl.Client.ReadNode(nodeId).As<T>();
        }
        catch
        {
            return fallback;
        }
    }

    // Status values 
    public int GetBatchId() => SafeRead("ns=6;s=::Program:Cube.Status.Parameter[0].Value", -1);
    public int GetAmount() => SafeRead("ns=6;s=::Program:Cube.Status.Parameter[1].Value", -1);
    public int GetPpm() => SafeRead("ns=6;s=::Program:Cube.Status.MachSpeed", -1);
    public float GetTemperature() => SafeRead("ns=6;s=::Program:Cube.Status.Parameter[3].Value", -1f);
    public decimal GetHumidity() => SafeRead("ns=6;s=::Program:Cube.Status.Parameter[2].Value", -1m);
    public decimal GetVibration() => SafeRead("ns=6;s=::Program:Cube.Status.Parameter[4].Value", -1m);
    public int GetDefects() => SafeRead("ns=6;s=::Program:Cube.Admin.ProdDefectiveCount", -1);
    public int GetProduced() => SafeRead("ns=6;s=::Program:Cube.Admin.ProdProcessedCount", -1);


    // Live success rate

    public int LiveSuccesRate()
    {
        var produced = GetProduced();
        var acceptable = GetAcceptable();

        try
        {
            return (acceptable * 100) / produced;
        }
        catch (DivideByZeroException)
        {
            return IsConnected() ? 100 : -1;
        }
    }


    private int InventoryPercentage(int inv)
    {
        try
        {
            double pct = inv / (double)MaxInventory * 100d;
            return (int)Math.Round(pct);
        }
        catch (DivideByZeroException)
        {
            return 0;
        }
    }

    // Inventory
    public int GetBarley() => InventoryPercentage(SafeRead("ns=6;s=::Program:Inventory.Barley", -1));
    public int GetHops() => InventoryPercentage(SafeRead("ns=6;s=::Program:Inventory.Hops", -1));
    public int GetMalt() => InventoryPercentage(SafeRead("ns=6;s=::Program:Inventory.Malt", -1));
    public int GetWheat() => InventoryPercentage(SafeRead("ns=6;s=::Program:Inventory.Wheat", -1));
    public int GetYeast() => InventoryPercentage(SafeRead("ns=6;s=::Program:Inventory.Yeast", -1));

    public int GetStatus() =>
        SafeRead("ns=6;s=::Program:Cube.Status.StateCurrent", -1);


    public BeerType GetCurrentBatch()
    {
        var current = SafeRead("ns=6;s=::Program:Cube.Admin.ProdProcessedCount.Value", -1);
        return Enum.IsDefined(typeof(BeerType), current) ? (BeerType)current : BeerType.Pilsner;
    }

    //Maintencance method
    public int GetMaintenanceStatus()
    {
        int counter = SafeRead("ns=6;s=::Program:Maintenance.Counter", 0);
        const int CYCLE = 30000;

        var pct = (counter % CYCLE) / (double)CYCLE * 100;
        return (int)Math.Round(pct);
    }


    public async Task<double> GetDefectRateAsync()
    {
        var total = GetProduced();
        var defects = GetDefects();

        return await Task.FromResult(total > 0 ? (defects / (double)total) * 100 : 0);
    }


    public async Task<int> GetAcceptableAsync()
    {
        var produced = GetProduced();
        var defects = GetDefects();

        return await Task.FromResult(produced - defects);
    }


    public int GetAcceptable()
    {
        var fallback = GetProduced() - GetDefects();
        var machineRead = SafeRead("ns=6;s=::Program:product.good", -1);

        return (machineRead > 0) ? machineRead : fallback;
    }


    public async Task<int> GetBatchProcessAsync()
    {
        var produced = GetProduced();
        var amount = GetAmount();

        return await Task.FromResult(amount > 0 ? (int)((produced / (double)amount) * 100) : 0);
    }

    public async Task<string> GetOnlineAsync()
    {
        return await Task.FromResult(MachineControl.IsConnected ? "Online" : "Offline");
    }

    //Command for OPC UA

    public async Task SetChangeRequestTrueAsync()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CmdChangeRequest", true);
        await Task.CompletedTask;
    }

    public async Task ResetCommandAsync()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 1);
        await SetChangeRequestTrueAsync();
    }

    public async Task StartCommandAsync()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 2);
        await SetChangeRequestTrueAsync();
    }

    public async Task StopCommandAsync()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 3);
        await SetChangeRequestTrueAsync();
    }

    public async Task AbortCommandAsync()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 4);
        await SetChangeRequestTrueAsync();
    }

    public async Task ClearCommandAsync()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 5);
        await SetChangeRequestTrueAsync();
    }

//auto checks the state and start machine depending on the state and command
    public async Task StartMachineAsync()
    {
        await Task.Delay(100);
        int statusVal = GetStatus();

        // Forbidden states
        if (statusVal is 0 or 3 or 6 or 11)
        {
            Console.WriteLine("Prevented start");
            Console.WriteLine(statusVal);
            return;
        }

        // Reset → Start flow
        if (statusVal is 2 or 5 or 17)
        {
            await ResetCommandAsync();
            await Task.Delay(1000);
            await StartCommandAsync();
        }
        // Ready-to-start state
        else if (statusVal == 4)
        {
            await StartCommandAsync();
        }
        // Error-clearing recovery flow
        else if (statusVal == 9)
        {
            await ClearCommandAsync();
            await Task.Delay(1000);
            await ResetCommandAsync();
            await Task.Delay(1000);
            await StartCommandAsync();
        }
        else
        {
            await Task.Delay(2000);
            await StartMachineAsync();
        }
    }

    //automated start
    public async Task AutomatedStart()
    {
        if (BatchQueue == null)
            throw new InvalidOperationException("BatchQueue is not available for AutomatedStart.");

        while (BatchQueue.Count > 0)
        {
            await Task.Delay(200);

            Batch nextBatch = BatchQueue.DequeueBatch();
            Console.WriteLine(
                $"Starting batch {nextBatch.Id} (Beer={nextBatch.BeerType}, Size={nextBatch.Size}), Speed={nextBatch.Speed}");

            await AddBatchAsync(nextBatch);
            await StartMachineAsync();

            // Wait for completion state 17
            while (GetStatus() != 17)
                await Task.Delay(500);

            Console.WriteLine($"Batch {nextBatch.Id} completed.");

            await ClearMachineAsync();
            await Task.Delay(500);
            await ResetMachineAsync();
            await Task.Delay(500);
        }

        Console.WriteLine("All batches processed.");
    }

    //methods for machine
    public async Task StopMachineAsync()
    {
        if (GetStatus() == 9) return;
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


    //Batch parameters
    public async Task AddBatchAsync(Batch batch)
    {
        // Each parameter is written individually because OPC UA batching is not used here
        await Task.Run(() =>
            MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.Parameter[0].Value", (float)batch.Id));

        await Task.Run(() =>
            MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.Parameter[1].Value", (float)batch.BeerType));

        await Task.Run(() =>
            MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.Parameter[2].Value", (float)batch.Size));

        await Task.Run(() =>
            MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.MachSpeed", batch.Speed));

        await SetChangeRequestTrueAsync();
    }
}