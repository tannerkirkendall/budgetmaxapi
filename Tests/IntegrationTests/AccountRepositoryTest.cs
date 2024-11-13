using Infrastructure;
using Microsoft.Extensions.Configuration;
using Moq;
using Tests.IntegrationTests.Common;


namespace Tests.IntegrationTests;

[TestClass]
public class AccountRepositoryTest
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
    public void TestCreateNewUserWithNewAccount()
    {
        var repo = new AccountRepository(_conf.Object);
        //repo.CreateNewAccountWithUser();

    }
    
}