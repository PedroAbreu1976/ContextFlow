namespace ContextFlow.Demo.GraphDemo;

public class RunFraudSignalScanGraphStep : DependencyGraphStep<LoanPreApprovalGraphContext>
{
    public override Task<bool> ExecuteAsync(LoanPreApprovalGraphContext context, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.FraudSignalScore = 82;
        context.ExecutedSteps.Add("Run fraud signal scan: success");

        return Task.FromResult(true);
    }
}
