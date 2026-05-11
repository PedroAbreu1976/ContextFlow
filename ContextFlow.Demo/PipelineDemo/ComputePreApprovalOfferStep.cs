namespace ContextFlow.Demo.PipelineDemo;

public class ComputePreApprovalOfferStep : IPipelineStep<LoanPreApprovalContext>
{
    public int Order => 50;

    public Task<bool> ExecuteAsync(LoanPreApprovalContext context, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.ExecutedSteps.Add(nameof(ComputePreApprovalOfferStep));

        context.IsEligible = context.CreditScore >= 640 && context.DebtToIncomeRatio <= 0.50m;
        context.ApprovedLimit = context.IsEligible
            ? Math.Round(context.AnnualIncome * 0.30m, 2)
            : 0m;

        context.InterestRate = context.RiskTier switch
        {
            "A" => 0.055m,
            "B" => 0.069m,
            "C" => 0.089m,
            _ => 0.120m
        };

        context.DecisionReasons.Add(context.IsEligible
            ? "Pre-approval offer computed."
            : "Applicant does not meet pre-approval thresholds.");

        return Task.FromResult(true);
    }
}
