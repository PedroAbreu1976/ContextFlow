namespace ContextFlow.Demo.GraphDemo;

public class GraphStep_Skip : GraphStepBase
{
    public override Type[] PreviousTypes => [typeof(GraphStep_1_1), typeof(GraphStep_2_1)];
}
