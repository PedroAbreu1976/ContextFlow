namespace ContextFlow.Demo.GraphDemo;

public class PersistApprovedDecisionGraphStep : DependencyGraphStep<LoanPreApprovalGraphContext, PullCreditScoreGraphStep, EvaluateWatchlistGraphStep>
{
    public override Task<bool> ExecuteAsync(LoanPreApprovalGraphContext context, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.DecisionPersisted = true;
        context.ExecutedSteps.Add("Persist approved decision: success");

        return Task.FromResult(true);
    }
}
