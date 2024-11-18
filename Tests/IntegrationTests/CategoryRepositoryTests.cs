﻿using Infrastructure;
using Microsoft.Extensions.Configuration;
using Moq;
using Tests.IntegrationTests.Common;

namespace Tests.IntegrationTests;

[TestClass]
public class CategoryRepositoryTests
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
    public void TestCanAddNewSubCategory()
    {
        var accountId = AccountRepositoryTestHelpers.CreateAccount();
        
        var catRepo = new CategoriesRepository(_conf.Object);
        var fakeCategory = new Random().Next(1000,99999).ToString();
        var categoryId = catRepo.CreateNewCategory(accountId, fakeCategory).Result;
        Assert.IsTrue(categoryId > 0);
        
        var fakeSubCategory = new Random().Next(1000,99999).ToString();
        var subCatId = catRepo.CreateNewSubCategory(accountId, categoryId, fakeSubCategory).Result;
        Assert.IsTrue(subCatId > 0);


        AccountRepositoryTestHelpers.DeleteAccount(accountId);
    }
}