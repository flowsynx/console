namespace Console.Models;

public class WorkflowExecutionUpdateDto
{
    public Guid WorkflowId { get; set; }
    public Guid ExecutionId { get; set; }
    public DateTime ExecutionStart { get; set; }
    public DateTime? ExecutionEnd { get; set; }
    public string Status { get; set; } = "";
}