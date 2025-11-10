using System.Reflection.PortableExecutable;
using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;

public class MachineControl
{
    public float Temperature { get; }
    public int BatchId { get; }
    public int Speed { get; }
    public bool AcceptedProduct { get; }
    public int QuantityAcceptedProduct { get; }

    public int status { get; }

    public string MachineUrl { get; }

    public int MachineId { get; }

    public OpcClient Client { get; set; }


    //Constructor 
    public MachineControl(int machineId, string machineUrl)
    {
        MachineId = machineId;
        MachineUrl = machineUrl;
        Client = new OpcClient(machineUrl);
    }

    public void Connect()
    {
        if (Client.State != OpcClientState.Connected)
        {
            Client.Connect();
        }
    }

}