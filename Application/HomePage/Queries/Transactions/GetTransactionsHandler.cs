using Application.Interfaces;
using Infrastructure;
using MediatR;

namespace Application.HomePage.Queries.Transactions;

public class GetTransactionsHandler : IRequestHandler<GetTransactionsRequest, GetTransactionsResult>
{
    private readonly IRepository _repo;
    private readonly ICurrentUserService _user;

    public GetTransactionsHandler(IRepository repo, ICurrentUserService user)
    {
        _repo = repo;
        _user = user;
    }

    public async Task<GetTransactionsResult> Handle(GetTransactionsRequest request, CancellationToken cancellationToken)
    {
        var transactions = await _repo.GetTransactionsByAccountId(_user.AccountId);
        var categories = (await _repo.GetCategoriesByAccountId(_user.AccountId)).ToList();
        var subCats = (await _repo.GetSubCategoriesByAccountId(_user.AccountId)).ToList();
        var returnObject = new GetTransactionsResult();
        
        foreach (var tran in transactions)
        {
            var subCat = subCats.FirstOrDefault(x => x.SubCategoryId == tran.SubCategoryId);
            var cat = categories.FirstOrDefault(x => x.CategoryId == subCat?.CategoryId);

            var transaction = new GetTransactionsResult.Transaction
            {
                TransactionId = tran.TransactionId,
                BankAccount = tran.BankAccount,
                TransactionDate = tran.TransactionDate,
                Amount = tran.Amount,
                Category = cat?.CategoryName ?? "",
                SubCategory = subCat?.SubCategoryName ?? "",
                SubCategoryId = tran.SubCategoryId,
                TransactionDescription = tran.TransactionDescription
            };
            
            returnObject.Transactions.Add(transaction);
        }

        return returnObject;
    }
}

public class GetTransactionsRequest : IRequest<GetTransactionsResult>
{

}

public class GetTransactionsResult
{
    public List<Transaction> Transactions { get; } = new();
    public class Transaction
    {
        public int TransactionId { get; init; }
        public string BankAccount { get; init; } = "";
        public DateTime TransactionDate { get; init; }
        public decimal Amount { get; init; }
        public string? Category { get; init; } 
        public string? SubCategory { get; init; } 
        public int SubCategoryId { get; init; }
        public string? TransactionDescription { get; init; }
    }
}