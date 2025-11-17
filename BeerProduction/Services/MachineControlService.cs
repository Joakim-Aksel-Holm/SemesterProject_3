using BeerProduction.Enums;

namespace BeerProduction.Services;

public class MachineControlService(MachineControl machineControl)
{
    public MachineControl MachineControl { get; set; } = machineControl;

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
        if (GetProduced() == 0)
        {
            return 0;
        }

        return (GetAmount() / GetProduced()) * 100;
    }

    /*public bool GetOnline()
    { 
        return true;
    }
    */

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

    public void SetChangeRequestTrue()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CmdChangeRequest", true);
    }

    // Reading the current status.
    public int GetStatus()
    {
        int status = MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent")
            .As<int>(); 
        return status;
    }

    // Basic method to Reset machine by directly writing the Reset command.
    public void ResetCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 1);
        SetChangeRequestTrue();
    }

    // Basic method to start machine by directly writing the start command.

    public void StartCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 2);
        SetChangeRequestTrue();
    }

    // Basic method to stop machine by directly writing the stop command.
    public void StopCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 3);
        SetChangeRequestTrue();
    }

    // Basic method to Abort machine by directly writing the Abort command.
    public void AbortCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 4);
        SetChangeRequestTrue();
    }

    // Basic method to Clear machine by directly writing the Clear command.
    public void ClearCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd", 5);
        SetChangeRequestTrue();
    }

    public void StartMachine() //prove of concept refactor
    {
        int statusVal = MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent").As<int>();

        if (statusVal == 0)
        {
            return;
        }

        else if (statusVal == 2)
        {
            ResetCommand();
            Thread.Sleep(1000); // wait one second to make sure that the command is recieved. 
            StartCommand();
        }
        else if (statusVal == 3)
        {
            return;
        }
        else if (statusVal == 4)
        {
            StartCommand();
        }
        else if (statusVal == 5)
        {
            ResetCommand();
            Thread.Sleep(1000); // wait one second to make sure that the command is recieved. 
            StartCommand();
        }
        else if (statusVal == 6)
        {
            return;
        }
        else if (statusVal == 9)
        {
            ClearCommand();
            Thread.Sleep(1000);
            ResetCommand();
            Thread.Sleep(1000);
            StartCommand();
        }
        else if (statusVal == 11)
        {
            return;
        }
        else if (statusVal == 17)
        {
            ResetCommand();
            Thread.Sleep(1000);
            StartCommand();
        }
        else
        {
            Thread.Sleep(2000);
            StartMachine();
        }
    }

    public void StopMachine()
    {
        int statusVal = MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent").As<int>();

        if (statusVal == 9)
        {
            return;
        }
        else
        {
            StopCommand();
        }
    }
}