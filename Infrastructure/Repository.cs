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
    private IDbConnection? _dbConn;
    private readonly string _connectionString;
    
    public Repository(IConfiguration config)
    {
        _connectionString = config.GetSection("ConnectionStrings:DefaultConnection").Value;
    }
    
    public async Task<IEnumerable<AppUser>> GetAppUserByEmail(string email)
    {
        Open();
        var appUser = await _dbConn.QueryAsync<AppUser>("SELECT email FROM AppUsers where UserId = 1");
        return appUser;
    }
    
    private void Open()
    {
        _dbConn ??= new SqlConnection(_connectionString);
        if (_dbConn.State != ConnectionState.Open)
            _dbConn.Open();
    }
    
    public void Dispose()
    {
        _dbConn?.Dispose();
    }
}