using ContextFlow;

namespace ContextFlow.Demo.ChainDemo;

public class EvaluateEligibilityStep : IChainOfResponsibilityStep<LoanPreApprovalChainContext>
{
    public int Order => 20;

    public async Task ExecuteAsync(LoanPreApprovalChainContext context, Func<LoanPreApprovalChainContext, CancellationToken?, Task> next, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.ExecutedSteps.Add(nameof(EvaluateEligibilityStep));

        var monthlyIncome = context.AnnualIncome / 12m;
        context.DebtToIncomeRatio = monthlyIncome == 0m ? 0m : context.MonthlyDebt / monthlyIncome;
        context.IsEligible = context.CreditScore >= 640 && context.DebtToIncomeRatio <= 0.50m;

        if (!context.IsEligible)
        {
            context.RiskTier = "D";
            context.ApprovedLimit = 0m;
            context.InterestRate = 0.120m;
            context.DecisionId = $"DEC-{Guid.NewGuid():N}";
            context.DecisionDateUtc = DateTimeOffset.UtcNow;
            context.DecisionReasons.Add("Chain stopped: applicant does not meet eligibility thresholds.");
            return;
        }

        context.DecisionReasons.Add($"Eligibility passed at DTI {context.DebtToIncomeRatio:P2}.");
        await next(context, ct);
    }
}
