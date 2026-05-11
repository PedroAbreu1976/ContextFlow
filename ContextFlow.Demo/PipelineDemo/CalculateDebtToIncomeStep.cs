namespace ContextFlow.Demo.PipelineDemo;

public class CalculateDebtToIncomeStep : IPipelineStep<LoanPreApprovalContext>
{
    public int Order => 20;

    public Task<bool> ExecuteAsync(LoanPreApprovalContext context, CancellationToken? ct = default)
    {
        ct?.ThrowIfCancellationRequested();

        context.ExecutedSteps.Add(nameof(CalculateDebtToIncomeStep));
        var monthlyIncome = context.AnnualIncome / 12m;
        context.DebtToIncomeRatio = monthlyIncome == 0m ? 0m : context.MonthlyDebt / monthlyIncome;
        context.DecisionReasons.Add($"Debt-to-income calculated at {context.DebtToIncomeRatio:P2}.");

        return Task.FromResult(true);
    }
}
