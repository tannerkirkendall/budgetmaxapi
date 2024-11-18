using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure;

public class CategoriesRepository : IDisposable
{
    private readonly NpgsqlConnection _sql;
    public CategoriesRepository(IConfiguration config)
    {
        _sql = new NpgsqlConnection(config.GetConnectionString("postgresqlConnection"));
    }

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