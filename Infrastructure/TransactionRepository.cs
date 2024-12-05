using System.Collections;
using System.Data;
using Application.Common.Interfaces;
using Dapper;
using Domain;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure;

public class TransactionRepository(IConfiguration config) : ITransactionRepository, IDisposable
{
    private readonly NpgsqlConnection _sql = new(config.GetConnectionString("postgresqlConnection"));

    public async Task<int> SaveNewTransaction(int accountId, string bankaccount, DateTime transactionDate, Decimal amount, 
        int subCategoryId, string transactionDescription)
    {
        Open();
        var param = new Dictionary<string, object>
        {
            {"accountid", accountId},
            {"bankaccount", bankaccount},
            {"transactiondate", transactionDate},
            {"amount", amount},
            {"subcategoryid", subCategoryId},
            {"transactiondescription", transactionDescription}
 
        };
        var sql = @"insert into transactions (accountid, bankaccount, transactiondate, amount, subcategoryid, transactiondescription)
            values (@accountid, @bankaccount, @transactiondate, @amount, @subcategoryid, @transactiondescription) RETURNING transactionid;";
        return await _sql.ExecuteScalarAsync<int>(sql,param);
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByAccountId(int accountId)
    {
        Open();
        var param = new Dictionary<string, object> {{"AccountId", accountId}};
        var sql = @"select transactionid, accountid, bankaccount, transactiondate, amount, subcategoryid, transactiondescription 
            from transactions where accountid = @accountId;";
        var result = await _sql.QueryAsync<Transaction>(sql, param);
        return result;
    }
    
    public async Task<IEnumerable<Transaction>> GetTransactionsByBudgetId(int accountId, int budgetId)
    {
        Open();
        var param = new Dictionary<string, object>
        {
            {"AccountId", accountId},
            {"budgetId", budgetId}
        };
        var sql = @"select transactionid, t.accountid, bankaccount, transactiondate, amount, subcategoryid, transactiondescription
                    from transactions t
                    inner join budgetheader bh
                    on t.transactiondate between bh.startdate and bh.enddate
                    and t.accountid = bh.accountid
                    where t.accountid = @accountId
                    and budgetid = @budgetId;";
        var result = await _sql.QueryAsync<Transaction>(sql, param);
        return result;
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