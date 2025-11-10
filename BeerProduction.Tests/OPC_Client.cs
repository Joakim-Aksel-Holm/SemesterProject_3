namespace BeerProduction.Tests;
using Xunit;
using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;
public class OPC_Client
{ 
    
    static MachineControl _testMachine = new MachineControl(1, "opc.tcp://192.168.0.122:4840"); 
    MachineControlService _testMachineControlService = new MachineControlService(_testMachine);
    public int status = _testMachine.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent").As<int>();


    [Fact]
    public void StartMachine()
    {
        // Arrange
        _testMachineControlService.StopMachine(); // Make sure that machine is stopped
        // Act
        _testMachineControlService.StartMachine(); // Start the machien
        status = _testMachine.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent").As<int>();
        // Assert that the status of the machine is executing and therefor the current status in 6. 
        Assert.Equal(6,status);
    }
    
    [Fact]
    public void StopMachine()
    {
        // Arrange
        _testMachineControlService.StartMachine(); // ensure that the machine is started
        // Act
        _testMachineControlService.StopMachine();
        status = _testMachine.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent").As<int>();

        // Assert that the current status of the machine is 2 and therefor stopped. 
        Assert.Equal(2,status);
    }

    [Fact]
    public void ChangeRequestTrue()
    {
        // Arrange 
        _testMachine.Client.WriteNode("ns=6;s=::Program:Cube.Command.CmdChangeRequest", false); // change request to false
        // Act 
        _testMachineControlService.SetChangeRequestTrue();

        // Assert 
        Assert.Equal(_testMachine.Client.ReadNode("ns=6;s=::Program:Cube.Command.CmdChangeRequest"),true);
    }

    [Fact]
    public void ChangeCMDNum1()
    {
        //Arrange
        _testMachine.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd" , 0); // set the value to something else.
        
        //Act 
       _testMachine.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd",1);
        
        //Assert
        Assert.Equal(_testMachine.Client.ReadNode("ns=6;s=::Program:Cube.Command.CntrlCmd"),1);
        
    }
    
}