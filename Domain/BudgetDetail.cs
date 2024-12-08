namespace Domain;

public class BudgetDetail
{
    public int BudgetId { get; set; }
    public int AccountId { get; set; }
    public int BudgetHeaderId { get; set; }
    public int SubCategoryId { get; set; }
    public decimal Amount { get; set; }
}