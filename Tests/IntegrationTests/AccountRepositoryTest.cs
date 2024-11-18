using Dapper;
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

        AccountRepositoryTestHelpers.DeleteAccount(emailQuery.AccountId);
    }
}

public static class AccountRepositoryTestHelpers
{
    public static int CreateAccount()
    {
        var sql = new NpgsqlConnection(StaticValues.DatabaseConnection);
        return sql.ExecuteScalar<int>("insert into accounts (accountname) VALUES ('test again') RETURNING accountid;");
    }

    public static void DeleteAccount(int accountId)
    {
        //delete records
        var sql = new NpgsqlConnection(StaticValues.DatabaseConnection);
        sql.Execute($"delete from subcategories where accountid = {accountId}");
        sql.Execute($"delete from categories where accountid = {accountId}");
        sql.Execute($"delete from appusers where accountid = {accountId}");
        sql.Execute($"delete from accounts where accountid = {accountId}");
    }
}

