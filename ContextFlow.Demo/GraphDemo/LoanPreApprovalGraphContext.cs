using System.Text;

namespace ContextFlow.Demo.GraphDemo;

public class LoanPreApprovalGraphContext : BaseContext
{
    public string ApplicantId { get; set; } = string.Empty;
    public decimal AnnualIncome { get; set; }
    public decimal MonthlyDebt { get; set; }
    public decimal DebtToIncomeRatio { get; set; }
    public int CreditScore { get; set; }
    public int FraudSignalScore { get; set; }
    public bool FraudPassed { get; set; }
    public bool IsEligible { get; set; }
    public decimal ApprovedLimit { get; set; }
    public decimal InterestRate { get; set; }
    public bool DecisionPersisted { get; set; }

    override public string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"{nameof(LoanPreApprovalGraphContext)}:");
        sb.AppendLine($"  ApplicantId: {ApplicantId}");
        sb.AppendLine($"  CreditScore: {CreditScore}");
        sb.AppendLine($"  DTI: {DebtToIncomeRatio:P2}");
        sb.AppendLine($"  FraudSignalScore: {FraudSignalScore}");
        sb.AppendLine($"  FraudPassed: {FraudPassed}");
        sb.AppendLine($"  IsEligible: {IsEligible}");
        sb.AppendLine($"  ApprovedLimit: {ApprovedLimit:C}");
        sb.AppendLine($"  InterestRate: {InterestRate:P2}");
        sb.AppendLine($"  DecisionPersisted: {DecisionPersisted}");
        sb.AppendLine("  ExecutedSteps:");
        ExecutedSteps.ForEach(step => sb.AppendLine($"  - {step}"));
        return sb.ToString();
    }
}
