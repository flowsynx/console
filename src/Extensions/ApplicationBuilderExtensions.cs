using Console.Middlewares;
using Console.Settings;

namespace Console.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseHttps(this IApplicationBuilder app)
    {
        var serviceProvider = app.ApplicationServices;
        var endpointConfiguration = serviceProvider.GetService<EndpointConfiguration>();
        if (endpointConfiguration != null && endpointConfiguration.Https?.Enabled == true)
        {
            app.UseHttpsRedirection();
        }

        return app;
    }

    public static IApplicationBuilder UseTokenExpiration(this IApplicationBuilder app)
    {
        app.UseMiddleware<TokenExpirationMiddleware>();
        return app;
    }
}