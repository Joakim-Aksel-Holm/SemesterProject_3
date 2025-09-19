using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;

var client = new OpcClient("opc.tcp://127.0.0.1:4840");
client.Connect();

var value = client.ReadNode("ns=2;s=Machine/Speed");
Console.WriteLine(value.Value);

client.Disconnect();