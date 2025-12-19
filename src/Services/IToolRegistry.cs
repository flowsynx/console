using Console.Models;

namespace Console.Services;

public interface IToolRegistry
{
    IReadOnlyCollection<ToolDescriptor> GetAll();
}