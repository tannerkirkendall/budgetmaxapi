using Domain;

namespace Application.Common.Interfaces;

public interface IBudgetRepository
{
    Task<int> CreateNewBudget(int accountId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<BudgetHeader>> GetBudgets(int accountId);
    Task<int> AddBudgetDetail(int accountId, int budgetId, int subCategoryId, decimal amount);
    Task<IEnumerable<BudgetDetail>> GetBudgetDetails(int accountId);
}