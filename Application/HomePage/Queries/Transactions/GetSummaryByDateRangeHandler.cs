using Application.Interfaces;
using Infrastructure;
using MediatR;

namespace Application.HomePage.Queries.Transactions;

public class GetSummaryByDateRangeHandler : IRequestHandler<GetSummaryByDateRangeRequest, GetSummaryByDateRangeResult>
{
    private readonly IRepository _repo;
    private readonly ICurrentUserService _user;

    public GetSummaryByDateRangeHandler(IRepository repo, ICurrentUserService user)
    {
        _repo = repo;
        _user = user;
    }
    public async Task<GetSummaryByDateRangeResult> Handle(GetSummaryByDateRangeRequest request, CancellationToken cancellationToken)
    {
        var transactions = await _repo.GetTransactionsByAccountId(_user.AccountId, request.StartDate, request.EndDate);
        var categories = (await _repo.GetCategoriesByAccountId(_user.AccountId)).ToList();
        var subCats = (await _repo.GetSubCategoriesByAccountId(_user.AccountId)).ToList();

        var returnObject = new GetSummaryByDateRangeResult();
        foreach (var transaction in transactions)
        {
            returnObject.Amount += transaction.Amount;
            
            var subCat = subCats.FirstOrDefault(x => x.SubCategoryId == transaction.SubCategoryId);
            var cat = categories.FirstOrDefault(x => x.CategoryId == subCat?.CategoryId);

            var catRoll = returnObject.Summary.FirstOrDefault(x => x.CategoryId == cat?.CategoryId);
            if (catRoll == null)
            {
                returnObject.Summary.Add(new GetSummaryByDateRangeResult.CategoryRoll
                {
                    Category = cat.CategoryName,
                    Amount = transaction.Amount,
                    CategoryId = cat.CategoryId,
                    SubCategories = new List<GetSummaryByDateRangeResult.SubCategoryRoll>
                    {
                        new GetSummaryByDateRangeResult.SubCategoryRoll
                        {
                            SubCategoryId = subCat.SubCategoryId,
                            SubCategory = subCat.SubCategoryName,
                            Amount = transaction.Amount,
                            Transactions = new List<GetSummaryByDateRangeResult.Transaction>
                            {
                                new GetSummaryByDateRangeResult.Transaction
                                {
                                    Amount = transaction.Amount,
                                    BankAccount = transaction.BankAccount,
                                    TransactionDate = transaction.TransactionDate,
                                    TransactionDescription = transaction.TransactionDescription,
                                    TransactionId = transaction.TransactionId
                                }
                            }
                        }
                    }
                });
            }
            else
            {
                catRoll.Amount += transaction.Amount;
                var subCatRoll = catRoll.SubCategories.FirstOrDefault(x => x.SubCategoryId == subCat?.SubCategoryId);
                if (subCatRoll == null)
                {
                    catRoll.SubCategories.Add(new GetSummaryByDateRangeResult.SubCategoryRoll
                    {
                        SubCategoryId = subCat.SubCategoryId,
                        SubCategory = subCat.SubCategoryName,
                        Amount = transaction.Amount,
                        Transactions = new List<GetSummaryByDateRangeResult.Transaction>
                        {
                            new GetSummaryByDateRangeResult.Transaction
                            {
                                Amount = transaction.Amount,
                                BankAccount = transaction.BankAccount,
                                TransactionDate = transaction.TransactionDate,
                                TransactionDescription = transaction.TransactionDescription,
                                TransactionId = transaction.TransactionId
                            }
                        }
                    });
                }
                else
                {
                    subCatRoll.Amount += transaction.Amount;
                    subCatRoll.Transactions.Add(
                        new GetSummaryByDateRangeResult.Transaction
                        {
                            Amount = transaction.Amount,
                            BankAccount = transaction.BankAccount,
                            TransactionDate = transaction.TransactionDate,
                            TransactionDescription = transaction.TransactionDescription,
                            TransactionId = transaction.TransactionId
                        });
                }
            }

        }

        return returnObject;
    }
}

public class GetSummaryByDateRangeRequest : IRequest<GetSummaryByDateRangeResult>
{
    public GetSummaryByDateRangeRequest(DateOnly startDate, DateOnly endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; }
}

public class GetSummaryByDateRangeResult
{
    public decimal Amount { get; set; }
    public List<CategoryRoll> Summary { get; set; } = new();
    public class CategoryRoll
    {
        public int CategoryId { get; init; }
        public string? Category { get; init; }
        public decimal Amount { get; set; }
        public List<SubCategoryRoll> SubCategories { get; init; } = new();
    }
    public class SubCategoryRoll
    {
        public string? SubCategory { get; init; } 
        public int SubCategoryId { get; init; }
        public decimal Amount { get; set; }
        public List<Transaction> Transactions { get; set; } = new();
    }
    public class Transaction
    {
        public int TransactionId { get; init; }
        public string BankAccount { get; init; } = "";
        public DateTime TransactionDate { get; init; }
        public decimal Amount { get; init; }
        public string? TransactionDescription { get; init; }
    }
}