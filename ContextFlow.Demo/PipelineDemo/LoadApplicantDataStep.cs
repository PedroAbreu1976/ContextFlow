namespace ContextFlow.Demo.PipelineDemo;

public class LoadApplicantDataStep : IPipelineStep<LoanPreApprovalPipelineContext>
{
    public int Order => 10;

    public Task<bool> ExecuteAsync(LoanPreApprovalPipelineContext context, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.ExecutedSteps.Add(nameof(LoadApplicantDataStep));
        context.ApplicantId = $"APP-{DateTime.UtcNow:yyyyMMddHHmmss}";
        context.AnnualIncome = 120_000m;
        context.MonthlyDebt = 1_800m;
        context.DecisionReasons.Add("Applicant data loaded from profile service.");

        return Task.FromResult(true);
    }
}
