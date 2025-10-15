using System.Text;
using BeerProduction.Components;
using Npgsql;
using Opc.UaFx;
using Opc.UaFx.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/run-diagnostics", (IConfiguration config) =>
{
    var log = new System.Text.StringBuilder();

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
    log.AppendLine("You are fat...Hot reload works!");

    return Results.Text(log.ToString(), "text/plain; charset=utf-8", Encoding.UTF8);
});
app.Run();
