namespace Console.Services;

public class ServerAccessTokenProvider : IAccessTokenProvider
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ITokenRefreshService _refreshService;

    public ServerAccessTokenProvider(
        IHttpContextAccessor contextAccessor,
        ITokenRefreshService refreshService)
    {
        _contextAccessor = contextAccessor;
        _refreshService = refreshService;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var context = _contextAccessor.HttpContext!;
        return await _refreshService.GetAccessTokenAsync(context);
    }
}