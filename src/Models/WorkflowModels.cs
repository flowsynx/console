using System.Text.Json.Serialization;

namespace Console.Services;

public class WorkflowViewModel
{
    public string? Schema { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public WorkflowConfiguration Configuration { get; set; } = new();
}

public class WorkflowContainer
{
    public string? Schema { get; set; }
    public WorkflowDefinition Workflow { get; set; } = new();
}

public class WorkflowDefinition
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Dictionary<string, object?>? Variables { get; set; } = new();
    public WorkflowConfiguration Configuration { get; set; } = new();
    public List<WorkflowTask> Tasks { get; set; } = new();
}

public class WorkflowConfiguration
{
    public int? DegreeOfParallelism { get; set; } = 3;
    public ErrorHandling? ErrorHandling { get; set; }
    public int? TimeoutMilliseconds { get; set; }
}

public class WorkflowTask
{
    [JsonIgnore] 
    public string NodeId { get; set; }
    public required string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public ExecutionConfig Execution { get; init; } = new();
    public FlowControlConfig FlowControl { get; init; } = new();
    public ErrorHandling? ErrorHandling { get; set; }
    public ManualApproval? ManualApproval { get; set; }
    public string? Output { get; set; } = string.Empty;
    public WorkflowTaskPosition? Position { get; set; } = new(0, 0);

    [JsonIgnore]
    public string? Status { get; set; }
}

public class ExecutionConfig
{
    public string Operation { get; set; } = string.Empty;
    public Dictionary<string, object?> Specification { get; set; } = new();
    public Dictionary<string, object?> Parameters { get; set; } = new();
    public AgentConfiguration? Agent { get; set; } = new();
    public int? TimeoutMilliseconds { get; set; }
}

public class AgentConfiguration
{
    public bool Enabled { get; set; }
    public string Mode { get; set; } = "execute"; // Agent mode: "execute" | "plan" | "validate" | "assist"
    public string? Instructions { get; set; }
    public int MaxIterations { get; set; } = 3;
    public double Temperature { get; set; } = 0.2;
    public Dictionary<string, object>? Context { get; set; } = new();
    public IEnumerable<string>? AllowTools { get; set; } = new List<string>();
    public IEnumerable<string>? DenyTools { get; set; } = new List<string>();
    public int MaxToolCalls { get; set; } = 6;
    public bool RequireToolApproval { get; set; }
    public bool DryRun { get; set; }
    public string? ToolSelection { get; set; } = "auto";
}

public class FlowControlConfig
{
    public List<string> Dependencies { get; set; } = new();
    public List<string> RunOnFailureOf { get; set; } = new();
    public Condition? ExecutionCondition { get; set; } = new();
    public List<ConditionalBranch> ConditionalBranches { get; set; } = new();
}

public class Condition
{
    public string Expression { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class ConditionalBranch
{
    public string Expression { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TargetTaskName { get; set; } = string.Empty;
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
    public TriggerPolicy? TriggerPolicy { get; set; } = new();
    public RetryPolicy? RetryPolicy { get; set; } = new();
}

public class TriggerPolicy
{
    public string? TaskName { get; set; }
}

public class RetryPolicy
{
    public int MaxRetries { get; set; } = 3;
    public string BackoffStrategy { get; set; } = "Fixed";
    public int InitialDelayMilliseconds { get; set; } = 1000;    // In millisecond
    public int MaxDelayMilliseconds { get; set; } = 10000;      // In millisecond
    public double BackoffCoefficient { get; set; } = 2.0;
}

public class ManualApproval
{
    public bool Enabled { get; set; } = false;
    public string Comment { get; set; } = string.Empty;
}