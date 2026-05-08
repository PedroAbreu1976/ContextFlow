namespace ContextFlow;

/// <summary>
/// Represents a step in a pipeline that can be executed asynchronously.
/// </summary>
/// <typeparam name="TContext">The type of context being processed.</typeparam>
public interface IPipelineStep<TContext> //: IPipelineStep
    where TContext : IContext
{
    int Order { get; }

    /// <summary>
    /// Executes this pipeline step for the specified context.
    /// </summary>
    /// <param name="context">The context being processed.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns><see langword="true"/> to continue pipeline execution; otherwise, <see langword="false"/>.</returns>
    Task<bool> ExecuteAsync(TContext context, CancellationToken? ct = default);
}

public abstract class PipelineStep<TContext>(int order) : IPipelineStep<TContext>
    where TContext : IContext
{
    public int Order => order;
    public abstract Task<bool> ExecuteAsync(TContext context, CancellationToken? ct = default);
}