namespace ContextFlow.Demo.GraphDemo;

public class PullCreditScoreGraphStep : DependencyGraphStep<LoanPreApprovalGraphContext, LoadApplicantProfileGraphStep>
{
    public override Task<bool> ExecuteAsync(LoanPreApprovalGraphContext context, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.CreditScore = 712;
        context.DebtToIncomeRatio = context.AnnualIncome == 0
            ? 0
            : Math.Round((context.MonthlyDebt * 12m) / context.AnnualIncome, 4);

        context.ExecutedSteps.Add("Pull credit score: success");

        return Task.FromResult(true);
    }
}
