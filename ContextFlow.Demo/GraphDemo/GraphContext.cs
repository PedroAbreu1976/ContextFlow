using System.Text;

namespace ContextFlow.Demo.GraphDemo;

public class GraphContext : BaseContext
{
    override public string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"{this.GetType().Name}:");
        ExecutedSteps.ForEach(step => sb.AppendLine($"  - {step}"));
        return sb.ToString();
    }
}
