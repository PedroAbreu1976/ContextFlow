// See https://aka.ms/new-console-template for more information
using ContextFlow;

namespace ContextFlow.Demo.ChainDemo;

public class ChainStep2 : IChainOfResponsibilityStep<ChainContext>
{
    public int Order => 2;

    public async Task ExecuteAsync(ChainContext context, Func<ChainContext, CancellationToken?, Task> next, CancellationToken? ct = default)
    {
        context.ExecutedSteps.Add(nameof(ChainStep2));
        await next(context, ct);
    }
}
