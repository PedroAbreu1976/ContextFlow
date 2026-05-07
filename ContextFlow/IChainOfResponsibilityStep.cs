namespace ContextFlow;

/// <summary>
/// Represents a step in a chain of responsibility that can invoke the next step.
/// </summary>
/// <typeparam name="TContext">The type of context being processed.</typeparam>
public interface IChainOfResponsibilityStep<TContext>
{
    int Order { get; }

    /// <summary>
    /// Executes this step and optionally invokes the next step in the chain.
    /// </summary>
    /// <param name="context">The context being processed.</param>
    /// <param name="next">The delegate to invoke the next step.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ExecuteAsync(TContext context, Func<TContext, CancellationToken?, Task> next, CancellationToken? ct = default);
}

public abstract class ChainOfResponsibilityStep<TContext>(int order) : IChainOfResponsibilityStep<TContext>
{
    public int Order => order;
    public abstract Task ExecuteAsync(TContext context, Func<TContext, CancellationToken?, Task> next, CancellationToken? ct = default);
}
