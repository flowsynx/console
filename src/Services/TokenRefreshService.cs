using Microsoft.AspNetCore.Authentication;
using System.Text.Json;
using Console.Settings;

namespace Console.Services;

public class TokenRefreshService : ITokenRefreshService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TokenRefreshService> _logger;
    private readonly OpenIdConnectConfiguration _oidcConfig;

    public TokenRefreshService(
        IHttpClientFactory httpClientFactory,
        ILogger<TokenRefreshService> logger,
        OpenIdConnectConfiguration oidcConfig)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _oidcConfig = oidcConfig;
    }

    public async Task<string?> GetAccessTokenAsync(HttpContext context)
    {
        var authenticateResult = await context.AuthenticateAsync();
        if (authenticateResult is null || !authenticateResult.Succeeded)
            return null;

        var expiresAt = authenticateResult.Properties?.GetTokenValue("expires_at");
        var accessToken = authenticateResult.Properties?.GetTokenValue("access_token");
        var refreshToken = authenticateResult.Properties?.GetTokenValue("refresh_token");

        if (DateTimeOffset.TryParse(expiresAt, out var exp) && exp > DateTimeOffset.UtcNow.AddMinutes(1))
            return accessToken;

        if (string.IsNullOrEmpty(refreshToken))
            return null;

        try
        {
            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_oidcConfig.Authority}/protocol/openid-connect/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string?>
                {
                    ["client_id"] = _oidcConfig.ClientId,
                    ["client_secret"] = _oidcConfig.ClientSecret,
                    ["grant_type"] = "refresh_token",
                    ["refresh_token"] = refreshToken
                }!)
            };

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to refresh token. Status {StatusCode}", response.StatusCode);
                return null;
            }

            var payload = await response.Content.ReadFromJsonAsync<JsonElement>();

            var newAccessToken = payload.GetProperty("access_token").GetString();
            var newRefreshToken = payload.TryGetProperty("refresh_token", out var rt)
                ? rt.GetString()
                : refreshToken;

            var expiresIn = payload.GetProperty("expires_in").GetInt32(); 
            var newExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresIn);

            authenticateResult.Properties!.UpdateTokenValue("access_token", newAccessToken);
            authenticateResult.Properties!.UpdateTokenValue("refresh_token", newRefreshToken);
            authenticateResult.Properties!.UpdateTokenValue("expires_at", newExpiresAt.ToString("o"));

            return newAccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception refreshing token");
            return null;
        }
    }
}