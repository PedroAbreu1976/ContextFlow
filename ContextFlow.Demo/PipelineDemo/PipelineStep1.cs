// See https://aka.ms/new-console-template for more information
using ContextFlow;

namespace ContextFlow.Demo.PipelineDemo;

public class PipelineStep1 : IPipelineStep<PipelineContext>
{
    public int Order => 1;

    public async Task<bool> ExecuteAsync(PipelineContext context, CancellationToken ct = default)
    {
        context.ExecutedSteps.Add(nameof(PipelineStep1));
        return true;
    }
}
