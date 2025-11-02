using Console.Components.Pages.Workflows;

namespace Console.Services;

public interface IWorkflowService
{
    WorkflowContainer Get();
    void Set(WorkflowContainer wf);
    string GetJson();
    void SetJson(string json);
    WorkflowContainer GetSample();
}