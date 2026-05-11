namespace ContextFlow.Demo.PipelineDemo;

public class AssignRiskTierStep : IPipelineStep<LoanPreApprovalContext>
{
    public int Order => 40;

    public Task<bool> ExecuteAsync(LoanPreApprovalContext context, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.ExecutedSteps.Add(nameof(AssignRiskTierStep));

        context.RiskTier = context.CreditScore switch
        {
            >= 760 => "A",
            >= 700 => context.DebtToIncomeRatio <= 0.35m ? "A" : "B",
            >= 650 => "C",
            _ => "D"
        };

        context.DecisionReasons.Add($"Risk tier assigned as {context.RiskTier}.");
        return Task.FromResult(true);
    }
}
