using csharp.SemesterProject_3;
using Npgsql;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace csharp;

class Program
{
    static void Main(string[] args)
    {
        // ---- OPC UA ----
        try
        {
            using (var client = new OpcClient("opc.tcp://127.0.0.1:4840"))
            {
                Console.WriteLine("🔌 Attempting to connect to OPC UA server...");

                client.Connect();
                Console.WriteLine("✅ Connected successfully!");

                var controlValue = client.ReadNode("ns=6;s=::Program:Cube.Command.CntrlCmd");
                Console.WriteLine("Værdi for control: " + controlValue);

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
        catch (OpcException ex)
        {
            Console.WriteLine("⚠️ Could not connect to OPC UA server: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Unexpected error: " + ex.Message);
        }

        // ---- PostgresSQL ----
        try
        {
            using var db = new DatabaseConnection();
            using var conn = db.GetConnection();
            using var cmd = new NpgsqlCommand("SELECT version();", conn);
            var version = cmd.ExecuteScalar();

            Console.WriteLine($"✅ Connected to PostgresSQL! Version: {version}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("⚠️ Database connection failed: " + ex.Message);
        }

        Console.WriteLine("\nProgram finished.");

        Console.WriteLine(" YOu are fat...Hot reload works!");
    }
}