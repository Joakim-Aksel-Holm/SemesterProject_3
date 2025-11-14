namespace BeerProduction.Tests;
using Xunit;
using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;
using System.Threading.Tasks;

public class OPC_Client
{ 
    
    static MachineControl _testMachine = new MachineControl(1, "opc.tcp://127.0.0.1:4840"); 
    MachineControlService _testMachineControlService = new MachineControlService(_testMachine);
    public int status = _testMachine.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent").As<int>();


    [Fact]
    public async Task StartMachine()
    {
        // Arrange
        await _testMachineControlService.StopMachineAsync(); // Make sure that machine is stopped
        Thread.Sleep(200);
        //add beers
        _testMachine.Client.WriteNode("ns=6;s=::Program:Cube.Command.Parameter[2].Value",100f); // add 100 beers
        Thread.Sleep(200);
        // Set speed 
        _testMachine.Client.WriteNode("ns=6;s=::Program:Cube.Command.MachSpeed",600f); // set speed 600 beer a minute
        Thread.Sleep(200);
        //set type to pilsner
        _testMachine.Client.WriteNode("ns=6;s=::Program:Cube.Command.Parameter[1].Value",0f);
        Thread.Sleep(200);
        
        // Act
        await _testMachineControlService.StartMachineAsync(); // Start the machine
        Thread.Sleep(200);
        status = _testMachine.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent").As<int>();
        // Assert that the status of the machine is executing and therefor the current status in 6. 
        Assert.Equal(6,status);
    }
    
    [Fact]
    public async Task StopMachine()
    {
        // Arrange
        
        // Add some beer to production
        await _testMachineControlService.StartMachineAsync(); // ensure that the machine is started
        // Act
        await _testMachineControlService.StopMachineAsync();
        Thread.Sleep(1000);
        status = _testMachine.Client.ReadNode("ns=6;s=::Program:Cube.Status.StateCurrent").As<int>();
        Thread.Sleep(1000);
        // Assert that the current status of the machine is 2 and therefor stopped. 
        Assert.Equal(2,status);
    }

    [Fact]
    public async Task ChangeRequestTrue()
    {
        // Arrange 
        _testMachine.Client.WriteNode("ns=6;s=::Program:Cube.Command.CmdChangeRequest", false); // change request to false
        Thread.Sleep(1000);
        // Act 
        await _testMachineControlService.SetChangeRequestTrueAsync();
        
        // Assert 
        bool testBool = _testMachine.Client.ReadNode("ns=6;s=::Program:Cube.Command.CmdChangeRequest").As<bool>();
        Assert.True(testBool);
    }
  

    [Fact]
    public void ChangeCMDNum1()
    {
        //Arrange
        _testMachine.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd" , 0); // set the value to something else.
        Thread.Sleep(1000);
        //Act 
       _testMachine.Client.WriteNode("ns=6;s=::Program:Cube.Command.CntrlCmd",1);
        Thread.Sleep(1000);
        //Assert
        Assert.Equal(_testMachine.Client.ReadNode("ns=6;s=::Program:Cube.Command.CntrlCmd").As<int>(),1);
        
    }
    
}