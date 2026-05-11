using ContextFlow;

namespace ContextFlow.Demo.ChainDemo;

public class ComputeOfferTermsStep : IChainOfResponsibilityStep<LoanPreApprovalChainContext>
{
    public int Order => 30;

    public async Task ExecuteAsync(LoanPreApprovalChainContext context, Func<LoanPreApprovalChainContext, CancellationToken?, Task> next, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.ExecutedSteps.Add(nameof(ComputeOfferTermsStep));

        context.RiskTier = context.CreditScore switch
        {
            >= 760 => "A",
            >= 700 => context.DebtToIncomeRatio <= 0.35m ? "A" : "B",
            >= 650 => "C",
            _ => "D"
        };

        context.ApprovedLimit = Math.Round(context.AnnualIncome * 0.30m, 2);
        context.InterestRate = context.RiskTier switch
        {
            "A" => 0.055m,
            "B" => 0.069m,
            "C" => 0.089m,
            _ => 0.120m
        };

        context.DecisionReasons.Add($"Offer computed for risk tier {context.RiskTier}.");
        await next(context, ct);
    }
}
