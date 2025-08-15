using Console.Components.Pages.Workflows;

namespace Console.Services;

public interface IWorkflowService
{
    Workflow Get();
    void Set(Workflow wf);
    string GetJson();
    void SetJson(string json);
    Workflow GetSample();
}