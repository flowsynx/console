using System.Text.Json.Serialization;

namespace Console.Services;

public class WorkflowViewModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public WorkflowConfiguration Configuration { get; set; } = new();
}

public class Workflow
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public WorkflowConfiguration Configuration { get; set; } = new();
    public List<WorkflowTask> Tasks { get; set; } = new();
}

public class WorkflowConfiguration
{
    public int? DegreeOfParallelism { get; set; } = 3;
    public ErrorHandling? ErrorHandling { get; set; }
    public int? Timeout { get; set; }
}

public class WorkflowTask
{
    [JsonIgnore] 
    public string NodeId { get; set; }
    public required string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public object? Type { get; set; }
    public Dictionary<string, object?> Parameters { get; set; } = new();
    public ErrorHandling? ErrorHandling { get; set; }
    public ManualApproval? ManualApproval { get; set; }
    public int? Timeout { get; set; }
    public List<string> Dependencies { get; set; } = new();
    public string? Output { get; set; } = string.Empty;
    public WorkflowTaskPosition? Position { get; set; } = new(0, 0);

    [JsonIgnore]
    public string? Status { get; set; }
}

public class WorkflowTaskPosition
{
    public WorkflowTaskPosition(double x, double y)
    {
        if (double.IsNaN(x) || double.IsInfinity(x))
            throw new ArgumentOutOfRangeException(nameof(x), "X must be a finite number.");
        if (double.IsNaN(y) || double.IsInfinity(y))
            throw new ArgumentOutOfRangeException(nameof(y), "Y must be a finite number.");

        X = x;
        Y = y;
    }

    public double X { get; set; }
    public double Y { get; set; }
}

public class ErrorHandling
{
    public string? Strategy { get; set; }
    public RetryPolicy? RetryPolicy { get; set; }
}

public class RetryPolicy
{
    public int MaxRetries { get; set; } = 3;
    public string BackoffStrategy { get; set; } = "Fixed";
    public int InitialDelay { get; set; } = 1000;    // In millisecond
    public int MaxDelay { get; set; } = 10000;      // In millisecond
    public double BackoffCoefficient { get; set; } = 2.0;
}

public class ManualApproval
{
    public bool Enabled { get; set; }
    public List<string> Approvers { get; set; } = new();
    public string Instructions { get; set; }
    public string DefaultAction { get; set; }
}