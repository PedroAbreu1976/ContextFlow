namespace ContextFlow.Demo.GraphDemo;

public class EvaluateWatchlistGraphStep : DependencyGraphStep<LoanPreApprovalGraphContext, RunFraudSignalScanGraphStep>
{
    public override Task<bool> ExecuteAsync(LoanPreApprovalGraphContext context, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.FraudPassed = context.FraudSignalScore < 80;
        context.ExecutedSteps.Add($"Evaluate watchlist: {(context.FraudPassed ? "success" : "blocked")}");

        return Task.FromResult(context.FraudPassed);
    }
}
