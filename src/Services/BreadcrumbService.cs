using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;

namespace Console.Services;

public class BreadcrumbService : IDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly List<BreadcrumbItem> _items = new();
    public IReadOnlyList<BreadcrumbItem> Items => _items;

    public event Action? OnChanged;

    public BreadcrumbService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += OnLocationChanged;
        BuildBreadcrumbs();
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        BuildBreadcrumbs();
        OnChanged?.Invoke();
    }

    private void BuildBreadcrumbs()
    {
        _items.Clear();

        var relativePath = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);
        if (string.IsNullOrEmpty(relativePath))
        {
            _items.Add(new BreadcrumbItem("Home", "/"));
            return;
        }

        var segments = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        string accumulated = "";
        _items.Add(new BreadcrumbItem("Home", "/"));

        for (int i = 0; i < segments.Length; i++)
        {
            accumulated += "/" + segments[i];
            var text = Capitalize(segments[i]);
            _items.Add(new BreadcrumbItem(text, accumulated, Disabled: i == segments.Length - 1));
        }
    }

    private static string Capitalize(string value) =>
        string.IsNullOrEmpty(value) ? value : char.ToUpper(value[0]) + value[1..];

    public void Dispose() =>
        _navigationManager.LocationChanged -= OnLocationChanged;
}

public record BreadcrumbItem(string Text, string Href, bool Disabled = false);