using System.Data;
using Dapper;
using Domain;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Infrastructure;

public interface IRepository
{
    public Task<IEnumerable<AppUser>> GetAppUserByEmail(string email);
    Task<IEnumerable<Category>> GetCategoriesByAccountId(int accountId);
    Task<IEnumerable<SubCategory>> GetSubCategoriesByAccountId(int accountId);
    Task<IEnumerable<Transaction>> GetTransactionsByAccountId(int accountId);
    Task<IEnumerable<Transaction>> GetTransactionsByAccountId(int accountId, DateOnly startDate, DateOnly endDate);
}

public class Repository : IRepository, IDisposable
{
    private readonly MySqlConnection _sql;

    public Repository(IConfiguration config)
    {
        var a = new MySqlConnection();
        a.ConnectionString = config.GetConnectionString("MySqlConnection");
        _sql = a;
    }
    
    public async Task<IEnumerable<AppUser>> GetAppUserByEmail(string email)
    {
        Open();
        var param = new Dictionary<string, object> {{"email", email}};
        var appUser = await _sql.QueryAsync<AppUser>(
            "SELECT UserId, AccountId, FirstName, HashedPassword, AccountEnabled, Email FROM AppUsers where email = @email", 
            param);
        return appUser;
    }
    
    public async Task<IEnumerable<Category>> GetCategoriesByAccountId(int accountId)
    {
        Open();
        var param = new Dictionary<string, object> {{"AccountId", accountId}};
        var appUser = await _sql.QueryAsync<Category>(
            "SELECT CategoryId, CategoryName FROM categories where AccountId = @accountId", 
            param);
        return appUser;
    }
    
    public async Task<IEnumerable<SubCategory>> GetSubCategoriesByAccountId(int accountId)
    {
        Open();
        var param = new Dictionary<string, object> {{"AccountId", accountId}};
        var appUser = await _sql.QueryAsync<SubCategory>(
            "SELECT SubCategoryId, CategoryId, SubCategoryName FROM subcategories where AccountId = @accountId", 
            param);
        return appUser;
    }
    
    public async Task<IEnumerable<Transaction>> GetTransactionsByAccountId(int accountId)
    {
        Open();
        var param = new Dictionary<string, object> {{"AccountId", accountId}};
        var appUser = await _sql.QueryAsync<Transaction>(
            @"SELECT TransactionId, AccountId, BankAccount, TransactionDate, Amount, SubCategoryId, TransactionDescription 
                FROM transactions where AccountId = @accountId", 
            param);
        return appUser;
    }
    
    public async Task<IEnumerable<Transaction>> GetTransactionsByAccountId(int accountId, DateOnly startDate, DateOnly endDate)
    {
        var startDateFormat = startDate.ToString("yyyy-MM-dd");
        var endDateFormat = endDate.ToString("yyyy-MM-dd");
        Open();
        var param = new Dictionary<string, object> {{"AccountId", accountId}, {"StartDate", startDateFormat}, {"EndDate", endDateFormat}};
        var appUser = await _sql.QueryAsync<Transaction>(
            @"SELECT TransactionId, AccountId, BankAccount, TransactionDate, Amount, SubCategoryId, TransactionDescription 
                FROM transactions 
                where AccountId = @accountId 
                and TransactionDate between @startDate and @endDate", 
            param);
        return appUser;
    }
    
    private void Open()
    {
        if (_sql.State != ConnectionState.Open)
            _sql.Open();
    }

    public void Dispose()
    {
       _sql.Dispose();
    }
}