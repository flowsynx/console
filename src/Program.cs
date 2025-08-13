using Console.Components;
using Console.Extensions;
using Console.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

var customConfigPath = builder.Configuration["config"];
if (!string.IsNullOrEmpty(customConfigPath))
{
    builder.Configuration.Sources.Clear(); // Optional: clear defaults
    builder.Configuration.AddJsonFile(customConfigPath, optional: false, reloadOnChange: false);
}

IConfiguration config = builder.Configuration;

builder.ConfigureHttpServer();

builder.Services
        .AddConsoleServices()
        .AddMudBlazor()
        .AddFlowSynxClientService(config);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOpenIdAuthentication(config);
builder.Services.AddAuthorizationCore();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttps();

app.UseAntiforgery();

app.UseAuthentication();
app.UseTokenExpiration();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapEndpoints();

app.Run();
