using Console.Settings;

namespace Console.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureHttpServer(this WebApplicationBuilder builder)
    {
        var endpointConfiguration = new EndpointConfiguration();
        builder.Configuration.GetSection("Endpoints").Bind(endpointConfiguration);
        builder.Services.AddSingleton(endpointConfiguration);

        using var scope = builder.Services.BuildServiceProvider().CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        ConfigureKestrelEndpoints(builder, endpointConfiguration, logger);
        ConfigureKestrelSettings(builder);

        return builder;
    }

    private static void ConfigureKestrelEndpoints(
        WebApplicationBuilder builder,
        EndpointConfiguration config,
        ILogger logger)
    {
        builder.WebHost.ConfigureKestrel((_, options) =>
        {
            var httpPort = config.Http?.Port ?? 6262;
            int? httpsPort = null;

            if (config.Https?.Enabled == true)
            {
                httpsPort = config.Https.Port ?? 6263;

                if (httpsPort == httpPort)
                    throw new InvalidOperationException($"HTTP and HTTPS endpoint ports cannot be the same: {httpPort}");
            }

            options.ListenAnyIP(httpPort);

            if (httpsPort.HasValue)
            {
                options.ListenAnyIP(httpsPort.Value, listenOptions =>
                {
                    var cert = config.Https!.Certificate;

                    if (cert != null)
                    {
                        if (!string.IsNullOrWhiteSpace(cert.Password))
                        {
                            listenOptions.UseHttps(cert.Path, cert.Password);
                        }
                        else
                        {
                            listenOptions.UseHttps(cert.Path);
                        }
                    }
                    else
                    {
                        listenOptions.UseHttps();
                    }
                });

                logger.LogInformation("Configuring HTTP and HTTPS endpoints: HTTP {HttpPort}, HTTPS {HttpsPort}", httpPort, httpsPort);
            }
            else
            {
                logger.LogInformation("Configuring HTTP endpoint only: HTTP {HttpPort}", httpPort);
            }
        });
    }

    private static void ConfigureKestrelSettings(WebApplicationBuilder builder)
    {
        builder.WebHost.UseKestrel(options =>
        {
            options.AddServerHeader = false;
            options.Limits.MaxRequestBufferSize = null;
        });
    }
}