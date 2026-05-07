namespace ContextFlow;

/// <summary>
/// Represents a step in a dependency graph with specified previous step types.
/// </summary>
/// <typeparam name="TContext">The type of context being processed.</typeparam>
public interface IDependencyGraphStep<TContext>
{
    Type[] PreviousTypes { get; }

    /// <summary>
    /// Executes this dependency graph step for the specified context.
    /// </summary>
    /// <param name="context">The context being processed.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns><see langword="true"/> when execution succeeds; otherwise, <see langword="false"/>.</returns>
    Task<bool> ExecuteAsync(TContext context, CancellationToken? ct = default);
}

public abstract class DependencyGraphStep<TContext> : IDependencyGraphStep<TContext>
{
    public virtual Type[] PreviousTypes => [];
    public abstract Task<bool> ExecuteAsync(TContext context, CancellationToken? ct = default);
}

public abstract class DependencyGraphStep<TContext, TDependencyGraphStep1> : DependencyGraphStep<TContext>
    where TDependencyGraphStep1 : IDependencyGraphStep<TContext>
{
    override public Type[] PreviousTypes => [typeof(TDependencyGraphStep1)];
}

public abstract class DependencyGraphStep<TContext, TDependencyGraphStep1, TDependencyGraphStep2> : DependencyGraphStep<TContext, TDependencyGraphStep1>
    where TDependencyGraphStep1 : IDependencyGraphStep<TContext>
    where TDependencyGraphStep2 : IDependencyGraphStep<TContext>
{
    override public Type[] PreviousTypes => [.. base.PreviousTypes, typeof(TDependencyGraphStep2)];
}

public abstract class DependencyGraphStep<TContext, TDependencyGraphStep1, TDependencyGraphStep2, TDependencyGraphStep3> : DependencyGraphStep<TContext, TDependencyGraphStep1, TDependencyGraphStep2>
    where TDependencyGraphStep1 : IDependencyGraphStep<TContext>
    where TDependencyGraphStep2 : IDependencyGraphStep<TContext>
    where TDependencyGraphStep3 : IDependencyGraphStep<TContext>
{
    override public Type[] PreviousTypes => [.. base.PreviousTypes, typeof(TDependencyGraphStep3)];
}

public abstract class DependencyGraphStep<TContext, TDependencyGraphStep1, TDependencyGraphStep2, TDependencyGraphStep3, TDependencyGraphStep4> : DependencyGraphStep<TContext, TDependencyGraphStep1, TDependencyGraphStep2, TDependencyGraphStep3>
    where TDependencyGraphStep1 : IDependencyGraphStep<TContext>
    where TDependencyGraphStep2 : IDependencyGraphStep<TContext>
    where TDependencyGraphStep3 : IDependencyGraphStep<TContext>
    where TDependencyGraphStep4 : IDependencyGraphStep<TContext>
{
    override public Type[] PreviousTypes => [.. base.PreviousTypes, typeof(TDependencyGraphStep4)];
}

public abstract class DependencyGraphStep<TContext, TDependencyGraphStep1, TDependencyGraphStep2, TDependencyGraphStep3, TDependencyGraphStep4, TDependencyGraphStep5> : DependencyGraphStep<TContext, TDependencyGraphStep1, TDependencyGraphStep2, TDependencyGraphStep3, TDependencyGraphStep4>
    where TDependencyGraphStep1 : IDependencyGraphStep<TContext>
    where TDependencyGraphStep2 : IDependencyGraphStep<TContext>
    where TDependencyGraphStep3 : IDependencyGraphStep<TContext>
    where TDependencyGraphStep4 : IDependencyGraphStep<TContext>
    where TDependencyGraphStep5 : IDependencyGraphStep<TContext>
{
    override public Type[] PreviousTypes => [.. base.PreviousTypes, typeof(TDependencyGraphStep5)];
}
