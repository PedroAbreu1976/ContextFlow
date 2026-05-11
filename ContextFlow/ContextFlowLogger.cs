using ContextFlow.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ContextFlow;

public class ContextFlowLogger(IServiceProvider serviceProvider, IOptions<ContextFlowConfiguration> options)
{
    public void Info<T>(T source, string message)
        where T : class
    {
        if (options.Value.LogExecution)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<T>>();
            logger.LogInformation(message);
        }
    }

    public void Warn<T>(T source, string message)
        where T : class
    {
        if (options.Value.LogExecution)
        {
            serviceProvider.GetRequiredService<ILogger<T>>()
                .LogWarning(message);
        }
    }

    public void Error<T>(T source, Exception error, string? message = default)
        where T : class
    {
        if (options.Value.LogExecution)
        {
            serviceProvider.GetRequiredService<ILogger<T>>()
                .LogError(error, message);
        }
    }
}
