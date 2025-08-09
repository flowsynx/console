using Microsoft.AspNetCore.Authentication;

namespace Console.Services;

public class ServerAccessTokenProvider : IAccessTokenProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServerAccessTokenProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        if (_httpContextAccessor.HttpContext == null)
            throw new InvalidOperationException("HttpContext is not available.");
        
        return await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
    }
}