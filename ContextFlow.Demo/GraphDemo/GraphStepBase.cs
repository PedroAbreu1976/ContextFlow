namespace ContextFlow.Demo.GraphDemo;

public abstract class GraphStepBase : IDependencyGraphStep<GraphContext>
{
    public virtual Type[] PreviousTypes => [];
    public virtual async Task<bool> ExecuteAsync(GraphContext context, CancellationToken? ct = null)
    {
        if(WaitTime > 0)
        {
            await Task.Delay(WaitTime, ct ?? CancellationToken.None);
        }
        context.ExecutedSteps.Add($"{this.GetType().Name}{(Proceed ? "" : " NOT")} executed.");
        return Proceed;
    }

    public virtual bool Proceed => true;

    public virtual int WaitTime => 500;
}
