using ContextFlow.Options;
using Microsoft.Extensions.Options;

namespace ContextFlow;

/// <summary>
/// Defines a pipeline for processing contexts through a series of steps.
/// </summary>
/// <typeparam name="TContext">The type of context being processed.</typeparam>
public interface IPipeline<TContext> where TContext : IContext
{
    /// <summary>
    /// Executes the pipeline for the specified context.
    /// </summary>
    /// <param name="context">The context being processed.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The processed context.</returns>
    Task<TContext> ExecuteAsync(TContext context, CancellationToken? ct = default);
}

/// <summary>
/// Implements a pipeline that executes steps in order until one fails or all complete.
/// </summary>
/// <typeparam name="TContext">The type of context being processed.</typeparam>
public class Pipeline<TContext>(IEnumerable<IPipelineStep<TContext>> steps) : IPipeline<TContext>
    where TContext : IContext
{
    /// <summary>
    /// Executes all pipeline steps for the specified context.
    /// </summary>
    /// <param name="context">The context being processed.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The processed context.</returns>
    public async Task<TContext> ExecuteAsync(TContext context, CancellationToken? ct = default)
    {
        if (!steps.Any())
        {
            throw new ArgumentException($"No steps found for context of type {context.GetType().Name}");
        }

        foreach (var step in steps.OrderBy(x => x.Order))
        {
            if(ct?.IsCancellationRequested == true)
            {
                break;
            }
            var result = await step.ExecuteAsync(context, ct ?? default);
            if (!result)
            {
                break;
            }
        }
        return context;
    }
}