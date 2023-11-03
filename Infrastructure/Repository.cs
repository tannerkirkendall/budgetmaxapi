using System.Data;
using System.Data.SqlClient;
using Dapper;
using Domain;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public interface IRepository
{
    public Task<IEnumerable<AppUser>> GetAppUserByEmail(string email);
}

public class Repository : IRepository, IDisposable
{
    private readonly IDbConnection _dbConn;

    public Repository(IConfiguration config)
    {
        _dbConn = new SqlConnection(config.GetConnectionString("DefaultConnection"));
    }
    
    public async Task<IEnumerable<AppUser>> GetAppUserByEmail(string email)
    {
        Open();
        var param = new Dictionary<string, object> {{"email", email}};
        var appUser = await _dbConn.QueryAsync<AppUser>(
            "SELECT UserId, AccountId, FirstName, HashedPassword, AccountEnabled, Email FROM AppUsers where email = @email", 
            param=param);
        return appUser;
    }
    
    private void Open()
    {
        if (_dbConn.State != ConnectionState.Open)
            _dbConn.Open();
    }

    public void Dispose()
    {
        _dbConn.Dispose();
    }
}