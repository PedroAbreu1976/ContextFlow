namespace ContextFlow.Demo.GraphDemo;

public class LoadApplicantProfileGraphStep : DependencyGraphStep<LoanPreApprovalGraphContext>
{
    public override Task<bool> ExecuteAsync(LoanPreApprovalGraphContext context, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.ApplicantId = "APP-1001";
        context.AnnualIncome = 120000m;
        context.MonthlyDebt = 2200m;
        context.ExecutedSteps.Add("Load applicant profile: success");

        return Task.FromResult(true);
    }
}
