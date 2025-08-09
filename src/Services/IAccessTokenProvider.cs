namespace Console.Services;

public interface IAccessTokenProvider
{
    Task<string?> GetAccessTokenAsync();
}
