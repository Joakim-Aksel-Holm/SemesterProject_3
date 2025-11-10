using System.Text;
using BeerProduction.Components;
using BeerProduction.Services;
using Npgsql;
using Opc.UaFx;
using Opc.UaFx.Client;

var builder = WebApplication.CreateBuilder(args);

// 🔽 Add this block so each dev's Local file is loaded (last wins)
builder.Configuration.AddJsonFile(
    $"appsettings.{builder.Environment.EnvironmentName}.Local.json",
    optional: true,
    reloadOnChange: true);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<DatabaseConnection>();
builder.Services.AddSingleton<BatchQueue>();

// Register OPC machine and service
builder.Services.AddSingleton<MachineControl>(sp =>
    new MachineControl(1, "opc.tcp://127.0.0.1:4840"));
builder.Services.AddSingleton<MachineControlService>();

var app = builder.Build();

// ✅ Try connecting to the machine safely using DI
using (var scope = app.Services.CreateScope())
{
    var machine = scope.ServiceProvider.GetRequiredService<MachineControl>();
    var machineService = scope.ServiceProvider.GetRequiredService<MachineControlService>();

    if (!machine.TryConnect())
    {
        Console.WriteLine("⚠️ Machine not connected, continuing without OPC UA.");
    }
    else
    {
        try
        {
            // Test StartMachineAsync
            Console.WriteLine("▶️ Testing StartMachineAsync...");
            machineService.StartMachineAsync();
            Console.WriteLine("✅ StartMachineAsync finished.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("⚠️ Error during machine test: " + ex.Message);
        }
    }
}


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

// Diagnostics endpoint
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

    // ---- PostgreSQL ----
    try
    {
        var cs = config.GetConnectionString("Default")
                 ?? throw new InvalidOperationException("Missing ConnectionStrings:Default");

        using var conn = new NpgsqlConnection(cs);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT version();", conn);
        var version = cmd.ExecuteScalar();
        log.AppendLine($"✅ Connected to PostgreSQL! Version: {version}");
    }
    catch (Exception ex)
    {
        log.AppendLine("⚠️ Database connection failed: " + ex.Message);
    }

    log.AppendLine();
    log.AppendLine("Program finished.");

    return Results.Text(log.ToString(), "text/plain; charset=utf-8", Encoding.UTF8);
});

// Simple ping endpoint for DB
app.MapGet("/api/pingdb", async (DatabaseConnection db, CancellationToken ct) =>
{
    await using var conn = await db.OpenAsync(ct);
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT 1";
    var result = await cmd.ExecuteScalarAsync(ct);
    return Results.Ok(new { ok = (int)result == 1 });
});

app.Run();
