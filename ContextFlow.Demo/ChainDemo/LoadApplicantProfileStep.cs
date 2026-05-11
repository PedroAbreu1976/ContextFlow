using ContextFlow;

namespace ContextFlow.Demo.ChainDemo;

public class LoadApplicantProfileStep : IChainOfResponsibilityStep<LoanPreApprovalChainContext>
{
    public int Order => 10;

    public async Task ExecuteAsync(LoanPreApprovalChainContext context, Func<LoanPreApprovalChainContext, CancellationToken?, Task> next, CancellationToken? ct = null)
    {
        ct?.ThrowIfCancellationRequested();

        context.ExecutedSteps.Add(nameof(LoadApplicantProfileStep));
        context.ApplicantId = string.IsNullOrWhiteSpace(context.ApplicantId)
            ? $"APP-{DateTime.UtcNow:yyyyMMddHHmmss}"
            : context.ApplicantId;

        context.AnnualIncome = context.AnnualIncome == 0m ? 120_000m : context.AnnualIncome;
        context.MonthlyDebt = context.MonthlyDebt == 0m ? 1_800m : context.MonthlyDebt;
        context.CreditScore = context.CreditScore == 0 ? 720 : context.CreditScore;
        context.DecisionReasons.Add("Applicant profile loaded for chain evaluation.");

        await next(context, ct);
    }
}
