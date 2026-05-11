namespace ContextFlow.Demo.ChainDemo;

public class LoanPreApprovalChainContext : BaseContext
{
    public string ApplicantId { get; set; } = string.Empty;
    public decimal AnnualIncome { get; set; }
    public decimal MonthlyDebt { get; set; }
    public int CreditScore { get; set; }
    public decimal DebtToIncomeRatio { get; set; }
    public string RiskTier { get; set; } = "Unknown";
    public decimal ApprovedLimit { get; set; }
    public decimal InterestRate { get; set; }
    public bool IsEligible { get; set; }
    public string DecisionId { get; set; } = string.Empty;
    public DateTimeOffset? DecisionDateUtc { get; set; }
    public List<string> DecisionReasons { get; } = [];

    public override string ToString()
    {
        var reasons = DecisionReasons.Count == 0 ? "none" : string.Join(" | ", DecisionReasons);
        return string.Join(Environment.NewLine,
        [
            $"{nameof(LoanPreApprovalChainContext)}:",
            $"  ApplicantId: {ApplicantId}",
            $"  CreditScore: {CreditScore}",
            $"  DTI: {DebtToIncomeRatio:P2}",
            $"  RiskTier: {RiskTier}",
            $"  IsEligible: {IsEligible}",
            $"  ApprovedLimit: {ApprovedLimit:C}",
            $"  InterestRate: {InterestRate:P2}",
            $"  DecisionId: {DecisionId}",
            $"  DecisionDateUtc: {DecisionDateUtc:O}",
            $"  Reasons: {reasons}",
            $"  Steps: {string.Join(", ", ExecutedSteps)}"
        ]);
    }
}
