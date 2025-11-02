using System.Text.Json;

namespace Console.Services;

public class WorkflowService: IWorkflowService
{
    private WorkflowContainer _workflow;

    public WorkflowService() 
    { 
        _workflow = GetSample(); 
    }

    public WorkflowContainer Get() => _workflow;

    public void Set(WorkflowContainer wf) => _workflow = wf;

    public string GetJson() => JsonSerializer.Serialize(_workflow, new JsonSerializerOptions { 
        WriteIndented = true,
        AllowTrailingCommas = true
    });

    public void SetJson(string json) 
    {
        var options = new JsonSerializerOptions { AllowTrailingCommas = true };
        _workflow = JsonSerializer.Deserialize<WorkflowContainer>(json, options) ?? new WorkflowContainer(); 
    }

    public WorkflowContainer GetSample()
    {
        var wf = new WorkflowContainer
        {
            Workflow = new WorkflowDefinition
            {
                Name = Guid.NewGuid().ToString(),
                Description = "This is a sample flowsynx workflow",
                Configuration = new WorkflowConfiguration { DegreeOfParallelism = 5, Timeout = 1000000, ErrorHandling = new ErrorHandling { Strategy = "Abort" } }
            }
        };
        wf.Workflow.Tasks.Add(new WorkflowTask
        {
            Name = "A",
            Type = "process",
            Parameters = new Dictionary<string, object?> {
                    { "FileName", "cmd.exe" },
                    { "Arguments", "/c echo Hello World!" },
                    { "ShowWindow", false },
                    { "FailOnNonZeroExit", true }
                },
            ErrorHandling = new ErrorHandling { Strategy = "Abort", RetryPolicy = new RetryPolicy { MaxRetries = 3, BackoffStrategy = "Fixed", InitialDelay = 1000, MaxDelay = 100 } },
            Timeout = 1000000,
            Position = new(80, 100),
            Output = "The log deleted."
        });
        wf.Workflow.Tasks.Add(new WorkflowTask
        {
            Name = "B",
            Type = "write",
            Parameters = new Dictionary<string, object?> {
                    { "Operation", "write" },
                    { "path", "D:/result.txt" },
                    { "Data", "$[Outputs('A')]" },
                    { "overwrite", true }
                },
            ErrorHandling = new ErrorHandling { Strategy = "Abort", RetryPolicy = new RetryPolicy { MaxRetries = 3, BackoffStrategy = "Fixed", InitialDelay = 1000, MaxDelay = 100 } },
            Timeout = 1000000,
            Dependencies = new List<string> { "A" },
            Position = new(380, 220)
        });
        return wf;
    }
}