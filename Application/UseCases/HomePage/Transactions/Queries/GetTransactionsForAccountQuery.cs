using Application.Common.Interfaces;
using Domain;
using MediatR;

namespace Application.UseCases.HomePage.Transactions.Queries;

public class GetTransactionsForAccountQueryHandler(ICurrentUserService userService, ITransactionRepository transactionRepository) 
    : IRequestHandler<GetTransactionsForAccountQuery, GetTransactionsForAccountResult>
{
    public async Task<GetTransactionsForAccountResult> Handle(GetTransactionsForAccountQuery request, CancellationToken cancellationToken)
    {
        var accountId = userService.AccountId;
        var result = await transactionRepository.GetTransactionsByAccountId(accountId);

        var resultSet = new List<GetTransactionsForAccountResult.TransactionResult>();
        foreach (var r in result)
        {
            resultSet.Add(new GetTransactionsForAccountResult.TransactionResult
            {
                AccountId = r.AccountId,
                TransactionId = r.TransactionId,
                Amount = r.Amount,
                BankAccount = r.BankAccount,
                TransactionDate = r.TransactionDate,
                TransactionDescription = r.TransactionDescription,
                SubCategoryId = r.SubCategoryId,
            });
            
        }
        
        return new GetTransactionsForAccountResult
        {
            Transactions = resultSet
        };
    }
}


public class GetTransactionsForAccountQuery : IRequest<GetTransactionsForAccountResult>
{
    
}


public class GetTransactionsForAccountResult
{
    public List<TransactionResult> Transactions { get; set; } = new();

    public class TransactionResult
    {
        public int TransactionId { get; init; }
        public int AccountId { get; init; }
        public string BankAccount { get; init; } = string.Empty;
        public DateTime TransactionDate { get; init; }
        public decimal Amount { get; init; }
        public int SubCategoryId { get; init; }
        public string TransactionDescription { get; init; } = string.Empty;
    }
}