using BeerProduction.Components;
using BeerProduction.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// 🔽 Add this block so each dev's Local file is loaded (last wins)
builder.Configuration.AddJsonFile(
    $"appsettings.{builder.Environment.EnvironmentName}.Local.json",
    optional: true, // OK if the file doesn't exist (e.g., CI/Prod)
    reloadOnChange: true); // nice for live edits during dev

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) .AddCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/login";
});
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddSingleton<DatabaseConnection>();
builder.Services.AddSingleton<BatchQueue>();
builder.Services.AddSingleton<ManagerService>();
builder.Services.AddSingleton<DatabaseConnection>();

builder.Services.AddScoped<ManagerService>(); 
builder.Services.AddScoped<MachineControlService>();
builder.Services.AddScoped(provider => new MachineControl(0, null, null));
builder.Services.AddScoped<AuthenticationStateService>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<AuthenticationStateService>());
builder.Services.AddScoped<BatchAnalysisService>();


var app = builder.Build();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();