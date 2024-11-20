using System.Data;
using Application.Interfaces;
using Dapper;
using Domain;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure;

public class CategoriesRepository(IConfiguration config) : ICategoriesRepository, IDisposable
{
    private readonly NpgsqlConnection _sql = new(config.GetConnectionString("postgresqlConnection"));

    public async Task<int> CreateNewCategory(int accountId, string categoryName)
    {
        Open();
        var param = new Dictionary<string, object>
        {
            {"accountid", accountId},
            {"categoryname", categoryName}
        };
        var sql =
            @"insert into categories (accountid, categoryname) 
                VALUES (@accountid, @categoryname) RETURNING categoryid;";
        return await _sql.ExecuteScalarAsync<int>(sql,param);
    }
    
    public async Task<int> CreateNewSubCategory(int accountId, int categoryId, string subCategoryName)
    {
        Open();
        var param = new Dictionary<string, object>
        {
            {"accountid", accountId},
            {"subcategoryname", subCategoryName},
            {"categoryid", categoryId}
        };
        var sql =
            @"insert into subcategories (accountid, categoryid, subcategoryname) 
                VALUES (@accountid, @categoryid, @subcategoryname) RETURNING subcategoryid;";
        return await _sql.ExecuteScalarAsync<int>(sql,param);
    }

    public async Task<IEnumerable<Category>> GetCategories(int accountId)
    {
        Open();
        var param = new Dictionary<string, object> {{"AccountId", accountId}};
        var categories = await _sql.QueryAsync<Category>(
            "SELECT categoryid, categoryname FROM categories where AccountId = @accountId", 
            param);
        return categories;
    }
    
    public async Task<IEnumerable<SubCategory>> GetSubCategories(int accountId)
    {
        Open();
        var param = new Dictionary<string, object> {{"AccountId", accountId}};
        var categories = await _sql.QueryAsync<SubCategory>(
            "SELECT subcategoryid, categoryid, subcategoryname FROM subcategories where AccountId = @accountId", 
            param);
        return categories;
    }

    public void Dispose()
    {
        _sql.Dispose();
    }
    
    private void Open()
    {
        if (_sql.State != ConnectionState.Open)
            _sql.Open();
    }
}