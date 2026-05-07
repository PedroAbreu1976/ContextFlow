namespace ContextFlow;

/// <summary>
/// Defines a chain of responsibility pattern for processing contexts.
/// </summary>
/// <typeparam name="TContext">The type of context being processed.</typeparam>
public interface IChainOfResponsibility<TContext>
    where TContext : IContext
{
    /// <summary>
    /// Executes the chain of responsibility for the specified context.
    /// </summary>
    /// <param name="context">The context being processed.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The processed context.</returns>
    Task<TContext> ExecuteAsync(TContext context, CancellationToken ct = default);
}

/// <summary>
/// Implements a chain of responsibility for executing steps in reverse order.
/// </summary>
/// <typeparam name="TContext">The type of context being processed.</typeparam>
public class ChainOfResponsibility<TContext>(IEnumerable<IChainOfResponsibilityStep<TContext>> steps) : IChainOfResponsibility<TContext>
    where TContext : IContext
{
    /// <summary>
    /// Executes all registered chain steps for the specified context.
    /// </summary>
    /// <param name="context">The context being processed.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The processed context.</returns>
    public async Task<TContext> ExecuteAsync(TContext context, CancellationToken ct = default)
    {
        if (steps == null || !steps.Any())
        {
            throw new ArgumentException($"No steps found for context of type {context.GetType().Name}");
        }

        Func<TContext, CancellationToken?, Task>? lastAction = null;
        foreach (var step in steps.OrderByDescending(x => x.Order))
        {
            var temp = lastAction;
            if (lastAction != null)
            {
                lastAction = (c, t) => step.ExecuteAsync(c, temp!, t);
            }
            else
            {
                lastAction = (c, t) => step.ExecuteAsync(c, (c, t) => Task.CompletedTask, t);
            }
        }

        await lastAction!(context, ct);
        
        return context;
    }
}


