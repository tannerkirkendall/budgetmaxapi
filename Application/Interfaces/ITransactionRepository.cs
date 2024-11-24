namespace Application.Interfaces;

public interface ITransactionRepository
{
    Task<int> SaveNewTransaction(int accountId, string bankaccount, DateTime transactionDate, Decimal amount,
        int subCategoryId, string transactionDescription);
}