namespace ContextFlow.Demo.PipelineDemo;

public class PullCreditScoreStep : IPipelineStep<LoanPreApprovalPipelineContext>
{
    public int Order => 30;

    public Task<bool> ExecuteAsync(LoanPreApprovalPipelineContext context, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.ExecutedSteps.Add(nameof(PullCreditScoreStep));
        context.CreditScore = 720;
        context.DecisionReasons.Add("Credit score pulled from bureau (simulated). ");

        return Task.FromResult(true);
    }
}
