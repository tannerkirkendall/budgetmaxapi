using System.Runtime.InteropServices.ComTypes;
using Application.Interfaces;
using MediatR;

namespace Application.HomePage.Queries.Transactions;

public abstract class GetSummaryByBudgetHeaderId
{
    public class Handler : IRequestHandler<Request, Result>
    {
        private readonly IRepository _repo;
        private readonly ICurrentUserService _user;

        public Handler(IRepository repo, ICurrentUserService user)
        {
            _repo = repo;
            _user = user;
        }

        public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            var budgetHeader = (await _repo.GetBudgetHeader(_user.AccountId, request.BudgetHeaderId)).First();
            var budgetDetails = (await _repo.GetBudgetDetails(_user.AccountId, request.BudgetHeaderId)).ToList();
            var transactions = await _repo.GetTransactionsByAccountId(_user.AccountId, budgetHeader.StartDate, budgetHeader.EndDate);
            var categories = (await _repo.GetCategoriesByAccountId(_user.AccountId)).ToList();
            var subCats = (await _repo.GetSubCategoriesByAccountId(_user.AccountId)).ToList();

            var returnObject = new Result();
            foreach (var transaction in transactions)
            {
                returnObject.Amount += transaction.Amount;

                var subCat = subCats.FirstOrDefault(x => x.SubCategoryId == transaction.SubCategoryId);
                var cat = categories.FirstOrDefault(x => x.CategoryId == subCat?.CategoryId);

                var catRoll = returnObject.Summary.FirstOrDefault(x => x.CategoryId == cat?.CategoryId);
                if (catRoll == null)
                {
                    returnObject.Summary.Add(new Result.CategoryRoll
                    {
                        Category = cat.CategoryName,
                        Amount = transaction.Amount,
                        CategoryId = cat.CategoryId,
                        SubCategories = new List<Result.SubCategoryRoll>
                        {
                            new Result.SubCategoryRoll
                            {
                                SubCategoryId = subCat.SubCategoryId,
                                SubCategory = subCat.SubCategoryName,
                                Amount = transaction.Amount,
                                Transactions = new List<Result.Transaction>
                                {
                                    new Result.Transaction
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
                    var subCatRoll =
                        catRoll.SubCategories.FirstOrDefault(x => x.SubCategoryId == subCat?.SubCategoryId);
                    if (subCatRoll == null)
                    {
                        catRoll.SubCategories.Add(new Result.SubCategoryRoll
                        {
                            SubCategoryId = subCat.SubCategoryId,
                            SubCategory = subCat.SubCategoryName,
                            Amount = transaction.Amount,
                            Transactions = new List<Result.Transaction>
                            {
                                new Result.Transaction
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
                            new Result.Transaction
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

    public class Request : IRequest<Result>
    {
        public Request(int budgetHeaderId)
        {
            BudgetHeaderId = budgetHeaderId;
        }

        public int BudgetHeaderId { get; }
    }

    public class Result
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
}