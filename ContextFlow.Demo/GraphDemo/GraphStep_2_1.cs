namespace ContextFlow.Demo.GraphDemo;

public class GraphStep_2_1 : GraphStepBase
{
    public override Type[] PreviousTypes => [typeof(GraphStep_2)];
    public override bool Proceed => false;
}
