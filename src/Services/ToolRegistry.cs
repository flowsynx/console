using Console.Models;
using FlowSynx.Client;

namespace Console.Services;

public class ToolRegistry : IToolRegistry
{
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IAccessTokenProvider _tokenProvider;
    private readonly ILogger<ToolRegistry> _logger;

    public ToolRegistry(
        IFlowSynxClient flowSynxClient, 
        IAccessTokenProvider tokenProvider,
        ILogger<ToolRegistry> logger)
    {
        _flowSynxClient = flowSynxClient;
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<ToolDescriptor>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var token = await _tokenProvider.GetAccessTokenAsync();
            _flowSynxClient.SetAuthenticationStrategy(
                new FlowSynx.Client.Authentication.BearerTokenAuthStrategy(token));

            var request = new FlowSynx.Client.Messages.Requests.Plugins.PluginsFullDetailsListRequest
            {
                Page = 1,
                PageSize = int.MaxValue
            };

            var result = await _flowSynxClient.Plugins.PluginsFullDetailsListAsync(request, cancellationToken);

            if (result.StatusCode != 200 || !result.Payload.Succeeded)
            {
                _logger.LogError("Failed to load plugins");
                return new List<ToolDescriptor>();
            }

            return result.Payload.Data
                .GroupBy(p => p.Type)
                .Select(g => new ToolDescriptor(g.Key, g.FirstOrDefault()?.Description))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new List<ToolDescriptor>();
        }
    }
}
