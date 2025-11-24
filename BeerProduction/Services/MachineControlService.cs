using System.Reflection.PortableExecutable;
using BeerProduction.Enums;
namespace BeerProduction.Services;
using System.Threading.Tasks;
using BeerProduction.Components.Model;
using BeerProduction.Enums;
using BeerProduction.Services;
using Opc.UaFx;
using Opc.UaFx.Client;

public class MachineControlService(MachineControl machineControl)
{
    public MachineControl MachineControl { get; } = machineControl;
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

    // Methods

    // Reads the Batch ID value
    public int GetMachineId()
    {
        return machineControl.MachineID;
    }

    // Reads name from table
    public string GetMachineName()
    {
        return machineControl.MachineName;
    }

    // Reads the ID of the current batch
    public int GetBatchId()
    {
        return MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.Parameter[0]").As<int>();
    }

    // Reads the Amount of products value
    public int GetAmount()
    {
        return MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.Parameter[1]").As<int>();
    }

    // Reads the Products per minute value
    public int GetPPM()
    {
        return MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.MachSpeed").As<int>();
    }

    // Reads the Temperature value
    public float GetTemperature()
    {
        return MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.Parameter[3]").As<float>();
    }

    // Reads the Humidity value
    public decimal GetHumidity()
    {
        return MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.Parameter[2]").As<decimal>();
    }

    // Reads the Vibration value
    public decimal GetVibration()
    {
        return MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.Parameter[4]").As<decimal>();
    }

    // Reads the Defected products value
    public int GetDefects()
    {
        return MachineControl.Client.ReadNode("ns=6;s=::Program:Admin.ProdDefectiveCount").As<int>();
    }

    //Defect rate method:
    public double GetDefectRate()
    {
        var total = GetProduced();
        return total > 0 ? (GetDefects() / (double)total) * 100 : 0;
    }
    // Method for Prouce Amount:


    // Reads the Accepted products value
    public int GetAcceptable()
    {
        return GetProduced() - GetDefects();
    }

    public int GetProduced()
    {
        return MachineControl.Client.ReadNode("ns=6;s=::Program:Admin.ProdProcessedCount").As<int>();
    }

    public int GetBatchProcess()
    {
        var produced = GetProduced();
        var amount = GetAmount();
        return amount > 0 ? (produced / amount) * 100 : 0;
    }

    public string GetOnline()
    {
        var serverStatus = false;
        if (MachineControl.IsConnected)
        {
            serverStatus = true;
        }

        return serverStatus ? "Online" : "Offline";
    }

    // Reads the value of current type and converts with enum
    public BeerType GetCurrentBatch()
    {
        var current = MachineControl.Client.ReadNode("ns=6;s=::Program:Admin.Parameter[0].Value").As<int>();

        if (Enum.IsDefined(typeof(BeerType), current))
        {
            return (BeerType)current;
        }

        return BeerType.Pilsner;
    }

    // Reads the Maintenance value
    public int GetMaintenanceStatus()
    {
        return MachineControl.Client.ReadNode("ns=6;s=::Program:Maintenance.Counter").As<int>();
    }

    public async Task SetChangeRequestTrueAsync()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CmdChangeRequest", true);
        await Task.CompletedTask;
    }

    // Reading the current status.
    public int GetStatus()
    {
        int status = MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent").As<int>();
        return status;
    }

    // Basic method to Reset machine by directly writing the Reset command.
    public async Task ResetCommandAsync()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 1);
        await SetChangeRequestTrueAsync();
    }

    // Basic method to start machine by directly writing the start command.

    public async Task StartCommandAsync()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 2);
         await SetChangeRequestTrueAsync();
    }

    // Basic method to stop machine by directly writing the stop command.
    public async Task StopCommandAsync()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 3);
         await SetChangeRequestTrueAsync();
    }

    // Basic method to Abort machine by directly writing the Abort command.
    public async Task AbortCommandAsync()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 4);
        await SetChangeRequestTrueAsync();
    }

    // Basic method to Clear machine by directly writing the Clear command.
    public async Task ClearCommandAsync()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 5);
        await SetChangeRequestTrueAsync();
    }

    public async Task StartMachineAsync()
    {
        int statusVal = GetStatus();

        if (statusVal == 0 || statusVal == 3 || statusVal == 6 || statusVal == 11)
            return;

        if (statusVal == 2 || statusVal == 5 || statusVal == 17)
        {
            await ResetCommandAsync();
            await Task.Delay(1000);
            await StartCommandAsync();
        }
        else if (statusVal == 4)
        {
            await StartCommandAsync();
        }
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
            await StartMachineAsync(); // recursive retry
        }
    }

    public async Task AutomatedStart()
    {
        while (BatchQueue.GetQueue().Count > 0)
        {
            Console.WriteLine("Vi er kommet til dequweuueue!!!");
            PriorityQueue<Batch,int> batchQueue = BatchQueue._batchQueue;

            Batch nextBatch = batchQueue.Dequeue();

            if (nextBatch == null)
                break; //Stop når der ikke er flere batches

            Console.WriteLine($"Starting batch {nextBatch.Id} (Beer={nextBatch.BeerType}, Size={nextBatch.Size})");


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

    public async Task QueueBatchMachineAsync(PriorityQueue<Batch, int> _batch)
    {
        await AddBatchAsync(_batch.Peek());
        _batch.Dequeue();
    }
}
