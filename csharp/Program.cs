using System;
using Opc.UaFx;
using Opc.UaFx.Client;
using Npgsql;

namespace SemesterProject_3.Services;

class Program
{
    static void Main(string[] args)
    {
			
        // ---- OPC UA ---- //
        try
        {
            using (var client = new OpcClient("opc.tcp://127.0.0.1:4840"))
            {
                client.Connect();

                var controlValue = client.ReadNode("ns=6;s=::Program:Cube.Command.CntrlCmd");
                Console.WriteLine("Værdi for control " + controlValue);

                var commands = new OpcWriteNode[]
                {
                    new OpcWriteNode("ns=6;s=::Program:Cube.Command.MachSpeed", 300.0f),
                    new OpcWriteNode("ns=6;s=::Program:Cube.Command.Parameter[1].Value", 2.0f),
                    new OpcWriteNode("ns=6;s=::Program:Cube.Command.Parameter[2].Value", 20.0f)
                };
                client.WriteNodes(commands);

                client.Disconnect();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ OPC UA error: " + ex.Message);
        }

        // ---- PostgresSQL ---- //
        try
        {
            using var db = new DatabaseConnection();  // uses your ctor with the connection string
            using var conn = db.GetConnection();
            using var cmd = new NpgsqlCommand("SELECT version();", conn);
            var version = cmd.ExecuteScalar();

            Console.WriteLine($"✅ Connected successfully! PostgreSQL version: {version}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ DB connection failed: {ex.Message}");
        }
			
    }
}
