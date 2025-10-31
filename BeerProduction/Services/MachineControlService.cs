using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Mvc;
using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;

public class MachineControlService
{


    public MachineControl MachineControl { get; set; }

    // contructor 
    public MachineControlService(MachineControl machineControl)
    {
        MachineControl = machineControl;
    }

    public void SetChangeRequestTrue()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CmdChangeRequest", true);
    }
    // methods
    public int GetStatus()
    {
        int status = MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent").As<int>(); // Reading the current status.
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