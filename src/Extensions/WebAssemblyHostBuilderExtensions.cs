using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Console.Extensions;

public static class WebAssemblyHostBuilderExtensions
{
    public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        return builder;
    }
}
