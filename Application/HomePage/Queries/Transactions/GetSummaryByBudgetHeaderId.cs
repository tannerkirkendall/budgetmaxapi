using System.Runtime.InteropServices.ComTypes;
using Application.Interfaces;
using Domain;
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
            var transactions = (await _repo.GetTransactionsByAccountId(_user.AccountId, budgetHeader.StartDate, budgetHeader.EndDate)).ToList();
            var subCategories = (await _repo.GetSubCategoriesByAccountId(_user.AccountId)).ToList();
            var categories = (await _repo.GetCategoriesByAccountId(_user.AccountId)).ToList();

            var result = new Result();
            
            foreach (var transaction in transactions)
            {
                var subCat = subCategories.FirstOrDefault(x => x.SubCategoryId == transaction.SubCategoryId);
                var cat = categories.FirstOrDefault(x => x.CategoryId == subCat?.CategoryId);

                result.NetTotal += transaction.Amount;
                var loopCat = result.Summary.FirstOrDefault(x => x.CategoryId == cat.CategoryId);
                if (loopCat == null)
                {
                    result.Summary.Add(new Result.CategoryRoll
                    {
                        CategoryId = cat.CategoryId,
                        Category = cat.CategoryName,
                        NetCategoryTotal = transaction.Amount,
                        SubCategories = new List<Result.SubCategoryRoll>
                        {
                            new Result.SubCategoryRoll
                            {
                                SubCategoryId = transaction.SubCategoryId,
                                SubCategory = subCat.SubCategoryName,
                                NetSubCategoryTotal = transaction.Amount,
                                Transactions = new List<Result.Transaction> {new(transaction)}
                            }
                        }
                    });
                }
                else
                {
                    loopCat.NetCategoryTotal += transaction.Amount;
                    var loopSubCat = loopCat.SubCategories.FirstOrDefault(x => x.SubCategoryId == transaction.SubCategoryId);
                    if (loopSubCat == null)
                    {
                        loopCat.SubCategories.Add(new Result.SubCategoryRoll
                        {
                            SubCategoryId = transaction.SubCategoryId,
                            SubCategory = subCat.SubCategoryName,
                            NetSubCategoryTotal = transaction.Amount,
                            Transactions = new List<Result.Transaction> {new(transaction)}

                        });
                    }
                    else
                    {
                        loopSubCat.NetSubCategoryTotal += transaction.Amount;
                        loopSubCat.Transactions.Add(new Result.Transaction(transaction));
                    }
                }
            }
            return result;
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
        public decimal NetTotal { get; set; }
        public List<CategoryRoll> Summary { get; set; } = new();

        public class CategoryRoll
        {
            public int CategoryId { get; init; }
            public string? Category { get; init; }
            public decimal NetCategoryTotal { get; set; }
            public List<SubCategoryRoll> SubCategories { get; init; } = new();
        }

        public class SubCategoryRoll
        {
            public string? SubCategory { get; init; }
            public int SubCategoryId { get; init; }
            public decimal NetSubCategoryTotal { get; set; }
            public List<Transaction> Transactions { get; set; } = new();
        }

        public class Transaction
        {
            public Transaction(Domain.Transaction tran)
            {
                TransactionId = tran.TransactionId;
                BankAccount = tran.BankAccount;
                TransactionDescription = tran.TransactionDescription;
                Amount = tran.Amount;
                TransactionDate = tran.TransactionDate;
            }
            public int TransactionId { get; }
            public string BankAccount { get; }
            public DateTime TransactionDate { get; }
            public decimal Amount { get; }
            public string? TransactionDescription { get; }
        }
    }
}