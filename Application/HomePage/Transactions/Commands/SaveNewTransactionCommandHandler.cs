using Application.Interfaces;
using MediatR;

namespace Application.HomePage.Transactions.Commands;

public class SaveNewTransactionCommandHandler(
    ICurrentUserService currentUserService,
    ITransactionRepository transactionRepository)
    : IRequestHandler<SaveNewTransactionCommand, SaveNewTransactionResult>
{
    public async Task<SaveNewTransactionResult> Handle(SaveNewTransactionCommand request, CancellationToken cancellationToken)
    {
        var accountId = currentUserService.AccountId;
        var result = await transactionRepository.SaveNewTransaction(accountId, request.BankAccountName, 
            request.Date, request.Amount ,request.SubCategoryId , request.TransactionDescription);
        return new SaveNewTransactionResult{TransactionId = result};
    }
}

public class SaveNewTransactionCommand : IRequest<SaveNewTransactionResult>
{
    public string BankAccountName { get; init; } = "";
    public DateTime Date { get; init; }
    public Decimal Amount { get; init; }
    public int SubCategoryId { get; init; }
    public string TransactionDescription { get; init; } = "";

}

public class SaveNewTransactionResult
{
    public int TransactionId { get; init; }
}