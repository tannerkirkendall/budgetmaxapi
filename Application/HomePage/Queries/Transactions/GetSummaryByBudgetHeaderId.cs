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
            
            return new Result();
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