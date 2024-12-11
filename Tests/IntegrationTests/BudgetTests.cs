using Infrastructure;
using Microsoft.Extensions.Configuration;
using Moq;
using Tests.IntegrationTests.Common;

namespace Tests.IntegrationTests;

[TestClass]
public class BudgetTests
{
    private Mock<IConfiguration> _conf = null!;
    
    [TestInitialize]
    public void TestSetup()
    {
        _conf = new Mock<IConfiguration>();
        var sect = new Mock<IConfigurationSection>();
        sect.SetupGet(m => m[It.Is<string>(s => s == "postgresqlConnection")]).Returns(StaticValues.DatabaseConnection);
        _conf.Setup(x => x.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(sect.Object);
    }

    [TestMethod]
    public void TestAddBudget()
    {
        var accountId = AccountRepositoryTestHelpers.CreateAccount();

        var budget = new BudgetRepository(_conf.Object);
        var budgetId1 = budget.CreateNewBudget(accountId, DateTime.Parse("2024-12-01"), DateTime.Parse("2025-01-04")).Result;
        var budgetId2 = budget.CreateNewBudget(accountId, DateTime.Parse("2024-09-01"), DateTime.Parse("2024-09-28")).Result;
        
        var budgets = (budget.GetBudgets(accountId).Result).ToList();
        
        Assert.AreEqual(budgetId1, budgets[0].BudgetId);
        Assert.AreEqual(accountId, budgets[0].AccountId);
        Assert.AreEqual(DateTime.Parse("2024-12-01"), budgets[0].StartDate);
        Assert.AreEqual(DateTime.Parse("2025-01-04"), budgets[0].EndDate);
        
        Assert.AreEqual(budgetId2, budgets[1].BudgetId);
        Assert.AreEqual(accountId, budgets[1].AccountId);
        Assert.AreEqual(DateTime.Parse("2024-09-01"), budgets[1].StartDate);
        Assert.AreEqual(DateTime.Parse("2024-09-28"), budgets[1].EndDate);
        
        AccountRepositoryTestHelpers.DeleteAccount(accountId);
    }
}