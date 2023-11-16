namespace Domain;

public class BudgetHeader
{
    public int BudgetHeaderId { get; set; }
    public int AccountId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}