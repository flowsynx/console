namespace Console.Services;

public interface ITokenRefreshService
{
    Task<string?> GetAccessTokenAsync(HttpContext context);
}