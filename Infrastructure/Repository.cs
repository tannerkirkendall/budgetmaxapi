using System.Data;
using Dapper;
using Domain;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Infrastructure;

public interface IRepository
{
    public Task<IEnumerable<AppUser>> GetAppUserByEmail(string email);
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
            param=param);
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