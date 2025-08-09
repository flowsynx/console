using Blazored.LocalStorage;
using Console.Preferences;
using Console.Services;
using Console.Settings;
using FlowSynx.Client.AspNetCore;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using MudBlazor.Services;

namespace Console.Extensions;

public static class ServiceCollectionExtensions
{
    private const string CookieScheme = "Cookies";
    private const string OpenIdScheme = "OpenIdConnect";

    public static IServiceCollection AddConsoleServices(this IServiceCollection services) =>
        services
            .AddBlazoredLocalStorage()
            .AddScoped<IPreferenceManager, PreferenceManager>()
            .AddHttpContextAccessor()
            .AddScoped<IAccessTokenProvider, ServerAccessTokenProvider>();

    public static IServiceCollection AddFlowSynxClientService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cliArguments = configuration.BindCliArguments();
        services.AddFlowSynxClient(opt =>
        {
            if (!string.IsNullOrWhiteSpace(cliArguments.Address))
            {
                opt.BaseUrl = cliArguments.Address;
            }
        });

        return services;
    }

    public static IServiceCollection AddMudBlazor(this IServiceCollection services) =>
        services.AddMudServices(ConfigureMudSnackbar);

    public static IServiceCollection AddOpenIdAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var openIdConfig = configuration
            .GetSection("OpenIdConnect")
            .Get<OpenIdConnectConfiguration>() ?? new();

        services.AddSingleton(openIdConfig);

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieScheme;
                options.DefaultChallengeScheme = OpenIdScheme;
            })
            .AddCookie(CookieScheme)
            .AddOpenIdConnect(OpenIdScheme, options =>
            {
                ConfigureOpenIdOptions(options, openIdConfig);
            });

        return services;
    }

    #region Private Helpers

    private static void ConfigureMudSnackbar(MudServicesConfiguration config)
    {
        var snackbar = config.SnackbarConfiguration;
        snackbar.PositionClass = Defaults.Classes.Position.BottomRight;
        snackbar.HideTransitionDuration = 500;
        snackbar.ShowTransitionDuration = 500;
        snackbar.VisibleStateDuration = 10000;
        snackbar.PreventDuplicates = false;
        snackbar.NewestOnTop = false;
        snackbar.ShowCloseIcon = true;
        snackbar.SnackbarVariant = Variant.Filled;
    }

    private static void ConfigureOpenIdOptions(
        OpenIdConnectOptions options,
        OpenIdConnectConfiguration config)
    {
        options.Authority = config.Authority;
        options.ClientId = config.ClientId;
        options.ClientSecret = config.ClientSecret;
        options.ResponseType = "code";
        options.CallbackPath = config.RedirectUri;
        options.SaveTokens = true;
        options.RequireHttpsMetadata = config.RequireHttps;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.GetClaimsFromUserInfoEndpoint = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name"
        };
    }

    #endregion
}