using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;
using System;

var client = new OpcClient("opc.tcp://127.0.0.1:4840");

client.Connect();

Console.WriteLine("Værdi for control " + client.ReadNode("ns=6;s=::Program:Cube.Command.CntrlCmd"));

// Produce 500 IPAs at 200 units of speed
OpcWriteNode[] commands = new OpcWriteNode[]
{
    new OpcWriteNode("ns=6;s=::Program:Cube.Command.MachSpeed", 300.0f),
    new OpcWriteNode("ns=6;s=::Program:Cube.Command.Parameter[1].Value", 2.0f),
    new OpcWriteNode("ns=6;s=::Program:Cube.Command.Parameter[2].Value", 20.0f)
};
client.WriteNodes(commands);

client.Disconnect();



