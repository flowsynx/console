using Console.Models;

namespace Console.Services;

public interface IToolRegistry
{
    Task<IReadOnlyCollection<ToolDescriptor>> GetAll(CancellationToken cancellationToken);
}