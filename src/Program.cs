using Blazored.LocalStorage;
using Console.Extensions;
using Console.Preferences;
using FlowSynx.Client.AspNetCore;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

WebAssemblyHost? app = null;

try
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.AddRootComponents();

    var configuration = new ConfigurationBuilder()
        .AddCommandLine(args)
        .Build();

    var cliArguments = configuration.BindCliArguments();

    builder.Services
            .AddBlazoredLocalStorage()
            .AddMudServices(configuration =>
            {
                configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                configuration.SnackbarConfiguration.HideTransitionDuration = 500;
                configuration.SnackbarConfiguration.ShowTransitionDuration = 500;
                configuration.SnackbarConfiguration.VisibleStateDuration = 10000;
                configuration.SnackbarConfiguration.PreventDuplicates = false;
                configuration.SnackbarConfiguration.NewestOnTop = false;
                configuration.SnackbarConfiguration.ShowCloseIcon = true;
                configuration.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            })
            .AddScoped<IPreferenceManager, PreferenceManager>()
            .AddFlowSynxClient(opt =>
            {
                if (!string.IsNullOrEmpty(cliArguments.Address))
                    opt.BaseUrl = cliArguments.Address;
            });

    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

    app = builder.Build();

    await app.RunAsync();
}
catch (Exception ex)
{
    if (app != null)
    {
        var logger = app.Services.GetRequiredService<ILogger>();
        logger.Log(LogLevel.Error, ex.Message);
    }
    else
    {
        System.Console.WriteLine(ex.Message);
    }
}