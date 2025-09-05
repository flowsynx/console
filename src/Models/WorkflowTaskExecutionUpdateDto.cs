namespace Console.Models;

public class WorkflowTaskExecutionUpdateDto
{
    public Guid WorkflowId { get; set; }
    public Guid ExecutionId { get; set; }
    public Guid TaskId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string Status { get; set; } = "";
}