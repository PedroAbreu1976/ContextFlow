namespace ContextFlow;

/// <summary>
/// Defines a dependency graph for processing contexts based on step dependencies.
/// </summary>
/// <typeparam name="TContext">The type of context being processed.</typeparam>
public interface IDependencyGraph<TContext>
    where TContext : IContext
{
    /// <summary>
    /// Executes the dependency graph for the specified context.
    /// </summary>
    /// <param name="context">The context being processed.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The processed context.</returns>
    Task<TContext> ExecuteAsync(TContext context, CancellationToken? ct = default);
}

/// <summary>
/// Implements a dependency graph that executes steps based on their dependencies.
/// </summary>
/// <typeparam name="TContext">The type of context being processed.</typeparam>
public class DependencyGraph<TContext>(IEnumerable<IDependencyGraphStep<TContext>> steps) : IDependencyGraph<TContext>
    where TContext : IContext
{
    /// <summary>
    /// Executes dependency graph steps for the specified context.
    /// </summary>
    /// <param name="context">The context being processed.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The processed context.</returns>
    public async Task<TContext> ExecuteAsync(TContext context, CancellationToken? ct = default)
    {
        if (steps == null || !steps.Any())
        {
            throw new ArgumentException($"No steps found for context of type {context.GetType().Name}");
        }

        var stepExecutions = steps.Select(s => new StepExecution(s)).ToList();
        foreach (var stepExecution in stepExecutions)
        {
            var dependentSteps = stepExecutions
                .Where(se => se.IsDependentOn(stepExecution.Step))
                .ToList();
            foreach (var dependentStep in dependentSteps)
            {
                stepExecution.OnStepExecuted += async (ctx) =>
                {
                    dependentStep.ParentExecuted(stepExecution);
                    if (dependentStep.CanExecute())
                    {
                        await dependentStep.ExecuteAsync(ctx, ct);
                    }
                };
            }
        }

        foreach (var stepExecution in stepExecutions.Where(se => se.HasNoDependencies()))
        {
            if(ct?.IsCancellationRequested == true)
            {
                break;
            }
            await stepExecution.ExecuteAsync(context, ct);
        }

        return context;
    }

    /// <summary>
    /// Encapsulates the execution state and logic for a dependency graph step.
    /// </summary>
    private class StepExecution
    {
        public event Func<TContext, Task>? OnStepExecuted;

        private List<StepExecution> _executedParents = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="StepExecution"/> class.
        /// </summary>
        /// <param name="step">The step metadata and executor.</param>
        public StepExecution(IDependencyGraphStep<TContext> step)
        {
            Step = step;
        }

        public IDependencyGraphStep<TContext> Step { get; }

        /// <summary>
        /// Executes the current step and triggers dependent steps when successful.
        /// </summary>
        /// <param name="context">The context being processed.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ExecuteAsync(TContext context, CancellationToken? ct)
        {
            if (ct?.IsCancellationRequested == true)
            {
                return;
            }
            var sucess = await Step.ExecuteAsync(context, ct);
            Executed = true;
            Sucess = sucess;
            if (sucess && OnStepExecuted != null)
            {
                await OnStepExecuted(context);
            }
        }

        public bool Executed { get; private set; } = false;
        public bool? Sucess { get; private set; }

        public bool ExecutedSuccessfully => Executed && Sucess == true;

        /// <summary>
        /// Determines whether this step can execute based on completed dependencies.
        /// </summary>
        /// <returns><see langword="true"/> when the step can execute; otherwise, <see langword="false"/>.</returns>
        public bool CanExecute()
        {
            return 
                Step.PreviousTypes == null || 
                Step.PreviousTypes.Length == 0 ||
                _executedParents.Count == Step.PreviousTypes.Length;
        }

        /// <summary>
        /// Records a parent step as executed.
        /// </summary>
        /// <param name="stepExecution">The executed parent step.</param>
        public void ParentExecuted(StepExecution stepExecution)
        {
            _executedParents.Add(stepExecution);
        }

        /// <summary>
        /// Determines whether this step depends on the specified step.
        /// </summary>
        /// <param name="otherStep">A potential dependency.</param>
        /// <returns><see langword="true"/> when this step depends on <paramref name="otherStep"/>; otherwise, <see langword="false"/>.</returns>
        public bool IsDependentOn(IDependencyGraphStep<TContext> otherStep)
        {
            return Step.PreviousTypes
                .Contains(otherStep.GetType());
        }

        /// <summary>
        /// Determines whether this step has no dependencies.
        /// </summary>
        /// <returns><see langword="true"/> when the step has no dependencies; otherwise, <see langword="false"/>.</returns>
        public bool HasNoDependencies()
        {
            return 
                Step.PreviousTypes == null || 
                Step.PreviousTypes.Length == 0;
        }
    }
}


