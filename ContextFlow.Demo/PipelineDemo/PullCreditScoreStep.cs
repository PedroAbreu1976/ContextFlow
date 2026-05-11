namespace ContextFlow.Demo.PipelineDemo;

public class PullCreditScoreStep : IPipelineStep<LoanPreApprovalContext>
{
    public int Order => 30;

    public Task<bool> ExecuteAsync(LoanPreApprovalContext context, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.ExecutedSteps.Add(nameof(PullCreditScoreStep));
        context.CreditScore = 720;
        context.DecisionReasons.Add("Credit score pulled from bureau (simulated). ");

        return Task.FromResult(true);
    }
}
