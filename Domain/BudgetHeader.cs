namespace Domain;

public class BudgetHeader
{
    public int BudgetId { get; init; }
    public int AccountId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}