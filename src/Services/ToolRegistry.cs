using Console.Models;

namespace Console.Services;

public class ToolRegistry : IToolRegistry
{
    private readonly List<ToolDescriptor> _tools = new()
    {
        new ToolDescriptor(
            Name: "FlowSynx.Storage.Local",
            Description: "Reads and writes files to local or mounted file systems. Supports directory traversal, file watching, and bulk file operations."
        ),
        new ToolDescriptor(
            Name: "FlowSynx.Data.Json",
            Description: "Loads and parses local JSON files. Supports transformation, extraction, and mapping of hierarchical data structures in workflows."
        ),
        new ToolDescriptor(
            Name: "FlowSynx.Data.Csv",
            Description: "Reads and writes CSV files, enabling easy batch data import/export operations and integration with spreadsheet-based data workflows."
        )
    };

    public IReadOnlyCollection<ToolDescriptor> GetAll() => _tools.AsReadOnly();
}
