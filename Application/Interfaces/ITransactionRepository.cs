namespace Application.Interfaces;

public interface ITransactionRepository
{
    Task<int> SaveNewTransaction(int accountId, string bankaccount, DateTime transactionDate, double amount,
        int subCategoryId, string transactionDescription);
}