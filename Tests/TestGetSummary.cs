using System.Reflection;
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

        var subCategories = GetSubCategories();
        mockRepo.Setup(x => x.GetSubCategoriesByAccountId(accountId)).ReturnsAsync(subCategories);

        var categories = GetCategories();
        mockRepo.Setup(x => x.GetCategoriesByAccountId(accountId)).ReturnsAsync(categories);
        
        var handler = new GetSummaryByBudgetHeaderId.Handler(mockRepo.Object, mockUserService.Object);
        var result = (handler.Handle(new GetSummaryByBudgetHeaderId.Request(budgetHeaderId), CancellationToken.None)).Result;
        mockUserService.VerifyAll();
        mockRepo.VerifyAll();
        
        Assert.IsInstanceOfType(result, typeof(GetSummaryByBudgetHeaderId.Result));
        
        Assert.AreEqual(40, result.NetTotal);
        Assert.AreEqual(2, result.Summary.Count);
        var categoryBills = result.Summary.Single(x => x.CategoryId == 1);
        Assert.AreEqual("Bills", categoryBills.Category);
        Assert.AreEqual(-60, categoryBills.NetCategoryTotal);
        Assert.AreEqual(2, categoryBills.SubCategories.Count);
        var billsWater = categoryBills.SubCategories.Single(y => y.SubCategoryId == 1);
        Assert.AreEqual("Water", billsWater.SubCategory);
        Assert.AreEqual("Electric", categoryBills.SubCategories.Single(y => y.SubCategoryId == 2).SubCategory);
        Assert.AreEqual(-40, billsWater.NetSubCategoryTotal);
        Assert.AreEqual(2, billsWater.Transactions.Count);
        Assert.AreEqual(-10, billsWater.Transactions.Single(x => x.TransactionId == 1).Amount);
        Assert.AreEqual(DateTime.Parse("2023-10-01"), billsWater.Transactions.Single(x => x.TransactionId == 1).TransactionDate);
        Assert.AreEqual("water bill", billsWater.Transactions.Single(x => x.TransactionId == 1).TransactionDescription);
        Assert.AreEqual("Ally", billsWater.Transactions.Single(x => x.TransactionId == 1).BankAccount);
        Assert.AreEqual(-30, billsWater.Transactions.Single(x => x.TransactionId == 4).Amount);
        Assert.AreEqual(DateTime.Parse("2023-10-04"), billsWater.Transactions.Single(x => x.TransactionId == 4).TransactionDate);
        Assert.AreEqual("second water bill", billsWater.Transactions.Single(x => x.TransactionId == 4).TransactionDescription);
        Assert.AreEqual("Chase", billsWater.Transactions.Single(x => x.TransactionId == 4).BankAccount);

    }

    private static List<Category> GetCategories()
    {
        return new List<Category>()
        {
            new()
            {
                CategoryId = 1,
                CategoryName = "Bills"
            },
            new()
            {
                CategoryId = 2,
                CategoryName = "Income"
            }
        };
    }

    private static List<SubCategory> GetSubCategories()
    {
        return new List<SubCategory>()
        {
            new ()
            {
                CategoryId = 1,
                SubCategoryId = 1,
                SubCategoryName = "Water"
            },
            new()
            {
                CategoryId = 1,
                SubCategoryId = 2,
                SubCategoryName = "Electric"
            },
            new()
            {
                CategoryId = 2,
                SubCategoryId = 3,
                SubCategoryName = "Paycheck"
            }
        };
    }
    
    private static List<Transaction> Transactions(int accountId)
    {
        var transactions = new List<Transaction>
        {
            new()
            {
                AccountId = accountId,
                Amount = -10,
                BankAccount = "Ally",
                TransactionDate = DateTime.Parse("2023-10-01"),
                TransactionDescription = "water bill",
                TransactionId = 1,
                SubCategoryId = 1
            },
            new()
            {
                AccountId = accountId,
                Amount = -20,
                BankAccount = "Ally",
                TransactionDate = DateTime.Parse("2023-10-02"),
                TransactionDescription = "electric bill",
                TransactionId = 2,
                SubCategoryId = 2
            },
            new()
            {
                AccountId = accountId,
                Amount = 100,
                BankAccount = "Ally",
                TransactionDate = DateTime.Parse("2023-10-03"),
                TransactionDescription = "tanner paycheck",
                TransactionId = 3,
                SubCategoryId = 3
            },
            new()
            {
                AccountId = accountId,
                Amount = -30,
                BankAccount = "Chase",
                TransactionDate = DateTime.Parse("2023-10-04"),
                TransactionDescription = "second water bill",
                TransactionId = 4,
                SubCategoryId = 1
            },
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
                Amount = 10
            }
        };
        return budgetDetails;
    }
}