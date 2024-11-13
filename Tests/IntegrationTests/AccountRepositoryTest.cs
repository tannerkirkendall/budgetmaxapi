using System.Data;
using Dapper;
using Domain;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Moq;
using Npgsql;
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
        var fakeEmail = new Random().Next(1000,99999).ToString();
        fakeEmail = $"{fakeEmail}@gmail.com";
        var emailQuery = repo.GetAppUserByEmail(fakeEmail).Result;
        Assert.IsNull(emailQuery);

        var accountResult = repo.CreateNewAccountWithUser("tanner", "kirk", fakeEmail, "fakePassword").Result;
        Assert.IsNotNull(accountResult);
        
        emailQuery = repo.GetAppUserByEmail(fakeEmail).Result;
        Assert.IsNotNull(emailQuery);
        
        Assert.AreEqual(accountResult.AccountId, emailQuery.AccountId);
        Assert.AreEqual(accountResult.UserId, emailQuery.UserId);
        Assert.AreEqual("tanner", emailQuery.FirstName);
        Assert.AreEqual("kirk", emailQuery.LastName);
        Assert.AreEqual("fakePassword", emailQuery.HashedPassword);
        Assert.AreEqual(fakeEmail, emailQuery.Email);
        
        //delete records
        var sql = new NpgsqlConnection(_conf.Object.GetConnectionString("postgresqlConnection"));
        sql.Execute($"delete from appusers where userid = {emailQuery.UserId}");
        sql.Execute($"delete from accounts where accountid = {emailQuery.AccountId}");
    }
    
}