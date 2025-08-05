using Console.Settings;
using Microsoft.Extensions.Options;

namespace Console.Extensions;

public static class CliArgumentBinderExtensions
{
    private class CliArgumentBinder<TOption> : IConfigureOptions<TOption> where TOption : class
    {
        private readonly IConfiguration _configuration;

        public CliArgumentBinder(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void Configure(TOption options)
        {
            var properties = typeof(TOption).GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                    continue;

                propertyInfo.SetValue(options, _configuration[propertyInfo.Name]);
            }
        }
    }

    public static CliArguments BindCliArguments(this IConfiguration configuration)
    {
        var cliArguments = new CliArguments();
        var cliArgumentBinder = new CliArgumentBinder<CliArguments>(configuration);
        cliArgumentBinder.Configure(cliArguments);
        return cliArguments;
    }
}