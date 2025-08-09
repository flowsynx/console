using Console.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Console.Endpoints;

public class Account : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapGet("/login", Login)
            .WithName("Login");

        group.MapGet("/logout", Logout)
            .WithName("Logout");
    }

    public async Task Login(HttpContext context, CancellationToken cancellationToken)
    {
        var returnUrl = context.Request.Query["returnUrl"].ToString();
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = "/";

        if (context.Request.Query["error"] == "access_denied")
        {
            throw new Exception("Authentication was canceled or denied.");
        }

        var props = new AuthenticationProperties
        {
            RedirectUri = returnUrl
        };

        await context.ChallengeAsync("OpenIdConnect", props);
    }


    public async Task Logout(HttpContext context, CancellationToken cancellationToken)
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await context.SignOutAsync("OpenIdConnect",
            new AuthenticationProperties
            {
                RedirectUri = "/"
            });
    }
}