using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Console.Services;

public class BreadcrumbService : IDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly List<BreadcrumbItem> _items = new();
    public IReadOnlyList<BreadcrumbItem> Items => _items;

    public event Action? OnChanged;

    private readonly List<string> _routeTemplates;

    public BreadcrumbService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += OnLocationChanged;

        _routeTemplates = GetAllRouteTemplates();
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
        _items.Add(new BreadcrumbItem("Home", "/"));

        var relativePath = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);
        if (string.IsNullOrEmpty(relativePath))
            return;

        var segments = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        string accumulated = "";

        for (int i = 0; i < segments.Length; i++)
        {
            accumulated += "/" + segments[i];
            var text = Capitalize(segments[i]);

            string? clickablePath = FindClickablePath(accumulated);

            bool disabled = clickablePath == null || i == segments.Length - 1;
            _items.Add(new BreadcrumbItem(text, clickablePath ?? accumulated, Disabled: disabled));
        }
    }

    private string? FindClickablePath(string path)
    {
        path = path.TrimEnd('/');
        var pathSegments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        foreach (var template in _routeTemplates)
        {
            var templateSegments = template.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (pathSegments.Length != templateSegments.Length)
                continue;

            bool matches = true;
            for (int i = 0; i < templateSegments.Length; i++)
            {
                if (IsParameterSegment(templateSegments[i]))
                    continue; // parameters can match anything

                if (!string.Equals(templateSegments[i], pathSegments[i], StringComparison.OrdinalIgnoreCase))
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
            {
                // build real path
                string builtPath = "/" + string.Join('/', pathSegments);
                return builtPath;
            }
        }

        return null;
    }

    private static bool IsParameterSegment(string segment) =>
        segment.StartsWith("{") && segment.EndsWith("}");

    private static List<string> GetAllRouteTemplates()
    {
        var assembly = Assembly.GetEntryAssembly()!;
        var routes = new List<string>();

        foreach (var type in assembly.ExportedTypes)
        {
            var routeAttributes = type.GetCustomAttributes<RouteAttribute>();
            foreach (var attr in routeAttributes)
            {
                routes.Add(attr.Template.TrimEnd('/'));
            }
        }

        return routes;
    }

    private static string Capitalize(string value) =>
        string.IsNullOrEmpty(value) ? value : char.ToUpper(value[0]) + value[1..];

    public void Dispose() =>
        _navigationManager.LocationChanged -= OnLocationChanged;
}

public record BreadcrumbItem(string Text, string Href, bool Disabled = false);