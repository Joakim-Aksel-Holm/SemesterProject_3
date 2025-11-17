using System.Reflection.PortableExecutable;
using System.Text;
using BeerProduction.Components;
using BeerProduction.Services;
using Npgsql;
using Opc.UaFx;
using Opc.UaFx.Client;


// it is working take 1
//hello
var builder = WebApplication.CreateBuilder(args);

// 🔽 Add this block so each dev's Local file is loaded (last wins)
builder.Configuration.AddJsonFile(
    $"appsettings.{builder.Environment.EnvironmentName}.Local.json",
    optional: true, // OK if the file doesn't exist (e.g., CI/Prod)
    reloadOnChange: true); // nice for live edits during dev

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<DatabaseConnection>();
builder.Services.AddSingleton<BatchQueue>();

builder.Services.AddScoped<ManagerService>(); 



var app = builder.Build();
try
{
    MachineControl machine1 = new MachineControl(1, "opc.tcp://192.168.0.122:4840", "Primary Brewer");
    MachineControlService machineService1 = new MachineControlService(machine1);
    int status = machineService1.GetStatus();
    Console.WriteLine("Machine 1 status: " + status);
    machineService1.StartMachine();
}
catch (Exception e)
{
    Console.WriteLine(e);
}

// Todo: shortcut the path: this could be a nice feature to figure out later on in the process.
//app.MapGet("/", ()=> Results.Redirect("/html/manager.html"));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();


app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/run-diagnostics", (IConfiguration config) =>
{
    var log = new StringBuilder();

    // ---- OPC UA ----
    try
    {
        using var client = new OpcClient("opc.tcp://127.0.0.1:4840");
        log.AppendLine("🔌 Attempting to connect to OPC UA server...");
        client.Connect();
        log.AppendLine("✅ Connected successfully!");

        var controlValue = client.ReadNode("ns=6;s=::Program:Cube.Command.CntrlCmd");
        log.AppendLine("Værdi for control: " + controlValue);

        var commands = new OpcWriteNode[]
        {
            new OpcWriteNode("ns=6;s=::Program:Cube.Command.MachSpeed", 300.0f),
            new OpcWriteNode("ns=6;s=::Program:Cube.Command.Parameter[1].Value", 2.0f),
            new OpcWriteNode("ns=6;s=::Program:Cube.Command.Parameter[2].Value", 20.0f)
        };

        client.WriteNodes(commands);
        client.Disconnect();
    }
    catch (OpcException ex)
    {
        log.AppendLine("⚠️ Could not connect to OPC UA server: " + ex.Message);
    }
    catch (Exception ex)
    {
        log.AppendLine("❌ Unexpected OPC error: " + ex.Message);
    }

    // ---- PostgresSQL ----
    try
    {
        var cs = config.GetConnectionString("Default")
                 ?? throw new InvalidOperationException("Missing ConnectionStrings:Default");

        using var conn = new NpgsqlConnection(cs);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT version();", conn);
        var version = cmd.ExecuteScalar();
        log.AppendLine($"✅ Connected to PostgresSQL! Version: {version}");
    }
    catch (Exception ex)
    {
        log.AppendLine("⚠️ Database connection failed: " + ex.Message);
    }

    log.AppendLine();
    log.AppendLine("Program finished.");


    return Results.Text(log.ToString(), "text/plain; charset=utf-8", Encoding.UTF8);
});


app.MapGet("/api/pingdb", async (DatabaseConnection db, CancellationToken ct) =>
{
    await using var conn = await db.OpenAsync(ct);
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT 1";
    var result = await cmd.ExecuteScalarAsync(ct);
    return Results.Ok(new { ok = (int)result == 1 });
});


app.Run();