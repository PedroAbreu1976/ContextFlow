namespace ContextFlow.Demo;

public abstract class BaseContext : IContext
{
    public List<string> ExecutedSteps { get; } = [];
    public override string ToString() => $"{this.GetType().Name}: {string.Join(", ", ExecutedSteps)}";
}
