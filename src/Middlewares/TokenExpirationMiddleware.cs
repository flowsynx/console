namespace Console.Middlewares;

public class TokenExpirationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenExpirationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var user = context.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var tokenProp = user.Claims.FirstOrDefault(c => c.Type == "access_token");

            if (tokenProp != null)
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(tokenProp.Value);
                if (jwt.ValidTo <= DateTime.UtcNow)
                {
                    context.Response.Redirect("/account/logout");
                }
            }
        }

        await _next(context);
    }
}