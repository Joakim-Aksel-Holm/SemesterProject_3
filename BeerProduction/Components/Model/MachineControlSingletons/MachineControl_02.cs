using System.Reflection.PortableExecutable;
using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;

public class MachineControl_02
{

    private static MachineControl_02 _instance = new(2, "opc.tcp://192.168.0.122:4840");
    public float Tempurature { get; }
    public int BatchId { get; }
    public int Speed { get; }
    public bool AcceptedProduct { get; }
    public int QuantityAcceptedProduct { get; }

    public int status { get; }

    public string MachineUrl { get; }

    public int MachineId { get; }

    public OpcClient Client { get; set; }


    //Constructor 
    private MachineControl_02(int machineId, string machineUrl)
    {
        MachineId = machineId;
        MachineUrl = machineUrl;
        Client = new OpcClient(machineUrl);
    }

    public static MachineControl_02 Instance => _instance;

    public void Connect()
    {
        if (Client.State != OpcClientState.Connected)
        {
            Client.Connect();
        }
    }

}