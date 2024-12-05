using Application.Common.Interfaces;
using MediatR;

namespace Application.UseCases.HomePage.Budgets.Commands;

public class AddNewBudgetCommandHandler(ICurrentUserService userService, IBudgetRepository budgetRepository)
    : IRequestHandler<AddNewBudgetCommand, AddNewBudgetCommandResult>
{
    public async Task<AddNewBudgetCommandResult> Handle(AddNewBudgetCommand request, CancellationToken cancellationToken)
    {
        var budgetid = await budgetRepository.CreateNewBudget(userService.AccountId, request.StartDate, request.EndDate);
        return new AddNewBudgetCommandResult
        {
            BudgetId = budgetid
        };
    }
}

public class AddNewBudgetCommand : IRequest<AddNewBudgetCommandResult>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class AddNewBudgetCommandResult
{
    public int BudgetId { get; init; }
}