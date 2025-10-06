using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;



class Program
{
    static void Main()
    {
        // Client connection
        
        
        
        // database connection test
        
        try
        {
            using var db = new DatabaseConnection();
            using var conn = db.GetConnection();

            using var cmd = new NpgsqlCommand("SELECT version();", conn);
            var version = cmd.ExecuteScalar();

            Console.WriteLine($"✅ Connected successfully! PostgreSQL version: {version}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Connection failed: {ex.Message}");
        }
    }
    
    }
