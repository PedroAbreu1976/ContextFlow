// See https://aka.ms/new-console-template for more information
using ContextFlow;

namespace ContextFlow.Demo.ChainDemo;

public class ChainStep1 : IChainOfResponsibilityStep<ChainContext>
{
    public int Order => 1;

    public async Task ExecuteAsync(ChainContext context, Func<ChainContext, CancellationToken?, Task> next, CancellationToken? ct = null)
    {
        context.ExecutedSteps.Add(nameof(ChainStep1));
        await next(context, ct);
    }
}
