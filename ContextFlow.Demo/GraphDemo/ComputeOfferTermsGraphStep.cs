namespace ContextFlow.Demo.GraphDemo;

public class ComputeOfferTermsGraphStep : DependencyGraphStep<LoanPreApprovalGraphContext, PullCreditScoreGraphStep>
{
    public override Task<bool> ExecuteAsync(LoanPreApprovalGraphContext context, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.IsEligible = context.CreditScore >= 680 && context.DebtToIncomeRatio <= 0.45m;

        context.ApprovedLimit = context.IsEligible
            ? Math.Round(context.AnnualIncome * 0.30m, 2)
            : 0m;

        context.InterestRate = context.CreditScore switch
        {
            >= 760 => 0.055m,
            >= 700 => 0.069m,
            >= 640 => 0.089m,
            _ => 0.12m
        };

        context.ExecutedSteps.Add("Compute offer terms: success");

        return Task.FromResult(true);
    }
}
