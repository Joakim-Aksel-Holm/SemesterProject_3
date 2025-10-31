using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Mvc;
using UaFx;
using UaFx.Client;

public class MachineControlService
{

    public MachineControl MachineControl { get; set; }

    // contructor 
    public MachineControlService(MachineControl machineControl)
    {
        MachineControl = machineControl;
    }


    // methods
    public int GetStatus()
    {
        int status = MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent").As<int>();
        return status;
    }
    
    // Basic method to Reset machine by directly writing the Reset command.
    public void ResetCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd",1);
    }

    // Basic method to start machine by directly writing the start command.

    public void StartCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd",2);
    }

     // Basic method to stop machine by directly writing the stop command.
     public void StopCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd",3);
    }

     // Basic method to Abort machine by directly writing the Abort command.
     public void AbortCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd",4);
    }
    
    // Basic method to Clear machine by directly writing the Clear command.
    public void ClearCommand()
    {
        MachineControl.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd",5);
    }





    public void StartMachine() //prove of concept refactor
    {
        int statusVal = MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent").As<int>();

        if (statusVal == 0)
        {
            return;
        }

        else if ((statusVal == 2))
        {
            // send komando, som resetter maskinen
            // send komando, som starter maskinen.
        }
        else if (statusVal == 3)
        {
            return;
        }
        else if (statusVal == 5)
        {
            return;
            // check de steps der skal til for at starte igen.
        }
        else if (statusVal == 6)
        {
            return;
        }
        else if (statusVal == 9)
        {
            // find de forskellige steps
        }
        else if (statusVal == 11)
        {
            // find de forskellige steps
        }
        else if (statusVal == 17)
        {
            // find the steps
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

        if (statusVal == 0)
        {
            return;
        }

        else if ((statusVal == 2))
        {
            // send komando, som resetter maskinen
            // send komando, som starter maskinen.
        }
        else if (statusVal == 3)
        {
            return;
        }
        else if (statusVal == 5)
        {
            return;
            // check de steps der skal til for at starte igen.
        }
        else if (statusVal == 6)
        {
            return;
        }
        else if (statusVal == 9)
        {
            // find de forskellige steps
        }
        else if (statusVal == 11)
        {
            // find de forskellige steps
        }
        else if (statusVal == 17)
        {
            // find the steps
        }
        else 
        {
            Thread.Sleep(2000);
            StartMachine();
        }
    }
 }