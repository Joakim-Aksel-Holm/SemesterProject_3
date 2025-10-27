using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Mvc;

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

    // lav metode, som resest maskinen

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
 }