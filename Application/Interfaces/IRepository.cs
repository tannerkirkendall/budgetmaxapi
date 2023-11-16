using Domain;

namespace Application.Interfaces;

public interface IRepository
{
    public Task<IEnumerable<AppUser>> GetAppUserByEmail(string email);
    Task<IEnumerable<Category>> GetCategoriesByAccountId(int accountId);
    Task<IEnumerable<SubCategory>> GetSubCategoriesByAccountId(int accountId);
    Task<IEnumerable<Transaction>> GetTransactionsByAccountId(int accountId);
    Task<IEnumerable<Transaction>> GetTransactionsByAccountId(int accountId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<BudgetHeader>> GetBudgetHeader(int accountId, int budgetHeaderId);
    Task<IEnumerable<BudgetDetail>> GetBudgetDetails(int accountId, int budgetHeaderId);
}