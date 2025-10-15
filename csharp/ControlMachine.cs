using Opc.UaFx;
using Opc.UaFx.Client;
class ControlMachine
{
    OpcClient client;

    //Using default for now
    string endpointUrl;
    public ControlMachine(string endpointURL = "opc.tcp://localhost:4840")
    {
        this.endpointUrl = endpointURL;
        client = new OpcClient(endpointUrl);
        client.Connect();
    }
}
