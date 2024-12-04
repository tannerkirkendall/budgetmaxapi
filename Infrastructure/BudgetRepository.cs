using System.Data;
using Application.Common.Interfaces;
using Dapper;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Npgsql;

namespace Infrastructure;

public class BudgetRepository(IConfiguration config) : IDisposable, IBudgetRepository
{
    private readonly NpgsqlConnection _sql = new(config.GetConnectionString("postgresqlConnection"));

    public async Task<int> CreateNewBudget(int accountId, DateTime startDate, DateTime endDate)
    {
        Open();
        var param = new Dictionary<string, object>
        {
            {"accountid", accountId},
            {"startdate", startDate},
            {"enddate", endDate}
            
        };
        var sql = "insert into budgetheader (accountid, startdate, enddate) values (@accountId, @startDate, @endDate) returning budgetid;";
        return await _sql.ExecuteScalarAsync<int>(sql, param);
    }

    public async Task<IEnumerable<BudgetHeader>> GetBudgets(int accountId)
    {
        Open();
        var sql = "select budgetid, accountid, startdate, enddate from budgetheader where accountid=@accountid order by startdate desc";
        return await _sql.QueryAsync<BudgetHeader>(sql, new { accountid = accountId });
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