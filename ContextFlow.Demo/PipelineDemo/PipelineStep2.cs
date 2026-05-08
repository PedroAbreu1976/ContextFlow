// See https://aka.ms/new-console-template for more information
using ContextFlow;

namespace ContextFlow.Demo.PipelineDemo;

public class PipelineStep2 : IPipelineStep<PipelineContext>
{
    public int Order => 2;

    public async Task<bool> ExecuteAsync(PipelineContext context, CancellationToken? ct = default)
    {
        context.ExecutedSteps.Add(nameof(PipelineStep2));
        return true;
    }
}
