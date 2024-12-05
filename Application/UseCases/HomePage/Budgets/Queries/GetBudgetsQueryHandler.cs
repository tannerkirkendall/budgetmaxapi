using Application.Common.Interfaces;
using MediatR;

namespace Application.UseCases.HomePage.Budgets.Queries;

public class GetBudgetsQueryHandler(ICurrentUserService userService, IBudgetRepository budgetRepository)
    : IRequestHandler<GetBudgetsQuery, GetBudgetsQueryResult>
{
    public async Task<GetBudgetsQueryResult> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
    {
        var budgets = await budgetRepository.GetBudgets(userService.AccountId);
        var result = new GetBudgetsQueryResult();
        foreach (var b in budgets)
        {
            result.Budgets.Add(new GetBudgetsQueryResult.Budget
            {
                BudgetId = b.BudgetId,
                EndDate = b.EndDate.ToShortDateString(),
                StartDate = b.StartDate.ToShortDateString(),
            });
        }
        result.Budgets = result.Budgets.OrderByDescending(b => b.StartDate).ToList();
        return result;
    }
}

public class GetBudgetsQuery : IRequest<GetBudgetsQueryResult>
{
    
}

public class GetBudgetsQueryResult
{
    public List<Budget> Budgets { get; set; } = new();
    public class Budget
    {
        public int BudgetId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}