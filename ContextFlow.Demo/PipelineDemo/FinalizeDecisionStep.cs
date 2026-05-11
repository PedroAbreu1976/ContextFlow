namespace ContextFlow.Demo.PipelineDemo;

public class FinalizeDecisionStep : IPipelineStep<LoanPreApprovalContext>
{
    public int Order => 60;

    public Task<bool> ExecuteAsync(LoanPreApprovalContext context, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.ExecutedSteps.Add(nameof(FinalizeDecisionStep));
        context.DecisionId = $"DEC-{Guid.NewGuid():N}";
        context.DecisionDateUtc = DateTimeOffset.UtcNow;
        context.DecisionReasons.Add("Decision finalized and persisted.");

        return Task.FromResult(true);
    }
}
