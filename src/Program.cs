using Blazored.LocalStorage;
using Console.Extensions;
using Console.Preferences;
using FlowSynx.Client.AspNetCore;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

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
            configuration.SnackbarConfiguration.HideTransitionDuration = 100;
            configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
            configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
            configuration.SnackbarConfiguration.ShowCloseIcon = false;
        })
        .AddScoped<IPreferenceManager, PreferenceManager>()
        .AddFlowSynxClient(opt =>
        {
            if (!string.IsNullOrEmpty(cliArguments.Address))
                opt.BaseUrl = cliArguments.Address;
        });

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();