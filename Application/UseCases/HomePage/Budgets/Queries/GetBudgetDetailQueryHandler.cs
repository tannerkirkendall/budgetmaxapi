using Application.Common.Interfaces;
using MediatR;

namespace Application.UseCases.HomePage.Budgets.Queries;

public class GetBudgetDetailQueryHandler (ICurrentUserService userService, IBudgetRepository budgetRepository, ICategoriesRepository categoriesRepository)
    : IRequestHandler<GetBudgetDetailQuery, GetBudgetDetailQueryResult>
{
    public async Task<GetBudgetDetailQueryResult> Handle(GetBudgetDetailQuery request, CancellationToken cancellationToken)
    {
        var budgetResults = await budgetRepository.GetBudgetDetails(userService.AccountId, request.BudgetId);
        var subCategories = (await categoriesRepository.GetSubCategories(userService.AccountId)).ToList();

        var result = new GetBudgetDetailQueryResult(request.BudgetId);
        
        foreach (var br in budgetResults)
        {
            result.BudgetDetails.Add(new GetBudgetDetailQueryResult.BudgetDetail
            {
                BudgeDetailId = br.BudgetDetailId,
                SubCategoryId = br.SubCategoryId,
                SubCategoryName = subCategories.FirstOrDefault(x => x.SubCategoryId == br.SubCategoryId)?.SubCategoryName ?? "none",
                Amount = br.Amount
            });
        }
        
        
        return result;
    }
}

public class GetBudgetDetailQuery(int budgetId) : IRequest<GetBudgetDetailQueryResult>
{
    public int BudgetId { get; init; } = budgetId;
}

public class GetBudgetDetailQueryResult(int budgetId)
{
    public int BudgetId { get; init; } = budgetId;
    public List<BudgetDetail> BudgetDetails { get; init; } = new();
    
    public class BudgetDetail
    {
        public int BudgeDetailId { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; } = "";
        public decimal Amount { get; set; }
    }
}