using System;
using System.Threading.Tasks;
using Opc.UaFx;
using Opc.UaFx.Client;

public class MachineControlService
{
    public MachineControl MachineControl { get; set; }

    public MachineControlService(MachineControl machineControl)
    {
        MachineControl = machineControl ?? throw new ArgumentNullException(nameof(machineControl));
    }

    // -------------------
    // Basic commands
    // -------------------
    public async Task SetChangeRequestTrueAsync()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CmdChangeRequest", true);
        await Task.CompletedTask; // async placeholder
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

    // -------------------
    // Status
    // -------------------
    public int GetStatus()
    {
        return MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent").As<int>();
    }

    // -------------------
    // High-level operations
    // -------------------
    public async Task StartMachineAsync()
    {
        Console.WriteLine("Starting machine...");

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

    public async Task StopMachineAsync()
    {
        Console.WriteLine("Stopping machine...");

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
}
