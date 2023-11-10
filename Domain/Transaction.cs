namespace Domain;

public class Transaction
{
    public int TransactionId { get; set; }
    public int AccountId { get; set; }
    public string BankAccount { get; set; } = "";
    public DateOnly TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public int SubCategoryId { get; set; }
    public string TransactionDescription { get; set; } = "";
}