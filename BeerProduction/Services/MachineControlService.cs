using System.Reflection.PortableExecutable;
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


    // methods
    public int GetStatus()
    {
        int status = MachineControl.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent").As<int>();
        return status;
    }
}