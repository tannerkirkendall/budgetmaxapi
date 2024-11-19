using Infrastructure;
using Microsoft.Extensions.Configuration;
using Moq;
using Tests.IntegrationTests.Common;

namespace Tests.IntegrationTests;

[TestClass]
public class TransactionsRepositoryTests
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
    public void TestCanAddNewTransaction()
    {
        var accountId = AccountRepositoryTestHelpers.CreateAccount();
        var catId = CategoryRepositoryTestsHelpers.CreateCategory(accountId, "bills");
        var subCatId = CategoryRepositoryTestsHelpers.CreateSubCategory(accountId, catId,"gas bill");
        
        var today = DateTime.Today;
        
        var transRepo = new TransactionRepository(_conf.Object);
        var tranId = transRepo.SaveNewTransaction(accountId, "discover", 
            today, (decimal)34.55, subCatId, "this was a log").Result;
        
        Assert.IsTrue(tranId > 0);


        AccountRepositoryTestHelpers.DeleteAccount(accountId);
    }
}