using Application.Common.Interfaces;
using MediatR;

namespace Application.UseCases.HomePage.Budgets.Commands;

public class AddNewBudgetDetailCommandHandler(ICurrentUserService userService, IBudgetRepository budgetRepository)
    : IRequestHandler<AddNewBudgetDetailCommand, AddNewBudgetDetailCommandResult>
{
    public async Task<AddNewBudgetDetailCommandResult> Handle(AddNewBudgetDetailCommand request, CancellationToken cancellationToken)
    {
        var result = await budgetRepository.AddBudgetDetail(userService.AccountId, request.BudgeId, request.SubCategoryId, request.Amount);
        
        return new AddNewBudgetDetailCommandResult
        {
            BudgetDetailId = result
        };
    }
}

public class AddNewBudgetDetailCommand : IRequest<AddNewBudgetDetailCommandResult>
{
    public int BudgeId { get; set; }
    public int SubCategoryId { get; set; }
    public decimal Amount { get; set; }
}

public class AddNewBudgetDetailCommandResult
{
    public int BudgetDetailId { get; init; }
}