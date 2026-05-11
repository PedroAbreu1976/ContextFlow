using ContextFlow;

namespace ContextFlow.Demo.ChainDemo;

public class FinalizeChainDecisionStep : IChainOfResponsibilityStep<LoanPreApprovalChainContext>
{
    public int Order => 40;

    public async Task ExecuteAsync(LoanPreApprovalChainContext context, Func<LoanPreApprovalChainContext, CancellationToken?, Task> next, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.ExecutedSteps.Add(nameof(FinalizeChainDecisionStep));
        context.DecisionId = string.IsNullOrWhiteSpace(context.DecisionId)
            ? $"DEC-{Guid.NewGuid():N}"
            : context.DecisionId;
        context.DecisionDateUtc ??= DateTimeOffset.UtcNow;
        context.DecisionReasons.Add("Decision finalized by chain.");

        await next(context, ct);
    }
}
