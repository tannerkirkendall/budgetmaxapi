using Application.HomePage.Queries.Transactions;
using Application.Interfaces;
using Domain;
using Moq;

namespace Tests;

[TestClass]
public class TestGetSummary
{
    [TestMethod]
    public void GetTransactionsInBudgetRange()
    {
        var mockRepo = new Mock<IRepository>();
        var mockUserService = new Mock<ICurrentUserService>();

        var accountId = new Random().Next(1, 100);
        var budgetHeaderId = new Random().Next(1, 100);
        
        mockUserService.Setup(x => x.AccountId).Returns(accountId);
        
        var budgetHeaders = BudgetHeaders(accountId, budgetHeaderId);
        mockRepo.Setup(x => x.GetBudgetHeader(accountId, budgetHeaderId)).ReturnsAsync(budgetHeaders);

        var budgetDetails = BudgetDetails(accountId, budgetHeaderId);
        mockRepo.Setup(x => x.GetBudgetDetails(accountId, budgetHeaderId)).ReturnsAsync(budgetDetails);

        var transactions = Transactions(accountId);
        mockRepo.Setup(x =>
                x.GetTransactionsByAccountId(accountId, budgetHeaders.First().StartDate, budgetHeaders.First().EndDate))
            .ReturnsAsync(transactions);
        
        var handler = new GetSummaryByBudgetHeaderId.Handler(mockRepo.Object, mockUserService.Object);
        var result = handler.Handle(new GetSummaryByBudgetHeaderId.Request(budgetHeaderId), CancellationToken.None);
        mockUserService.VerifyAll();
        mockRepo.VerifyAll();
    }

    private static List<Transaction> Transactions(int accountId)
    {
        var transactions = new List<Transaction>
        {
            new Transaction
            {
                AccountId = accountId
            }
        };
        return transactions;
    }

    private static List<BudgetHeader> BudgetHeaders(int accountId, int budgetHeaderId)
    {
        var budgetHeaders = new List<BudgetHeader>
        {
            new BudgetHeader
            {
                AccountId = accountId,
                BudgetHeaderId = budgetHeaderId,
                StartDate = DateTime.Parse("2023-10-01"),
                EndDate = DateTime.Parse("2023-10-31")
            }
        };
        return budgetHeaders;
    }

    private static List<BudgetDetail> BudgetDetails(int accountId, int budgetHeaderId)
    {
        var budgetDetails = new List<BudgetDetail>
        {
            new BudgetDetail
            {
                AccountId = accountId,
                BudgetHeaderId = budgetHeaderId,
                BudgetDetailId = 1,
                SubCategoryId = 1,
                Amount = 100
            }
        };
        return budgetDetails;
    }
}