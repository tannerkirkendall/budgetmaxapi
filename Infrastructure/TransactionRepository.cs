using System.Data;
using Application.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure;

public class TransactionRepository(IConfiguration config) : ITransactionRepository, IDisposable
{
    private readonly NpgsqlConnection _sql = new(config.GetConnectionString("postgresqlConnection"));

    public async Task<int> SaveNewTransaction(int accountId, string bankaccount, DateTime transactionDate, double amount, 
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