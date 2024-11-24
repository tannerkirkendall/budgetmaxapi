using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Application.Common.Interfaces;
using Domain;
using Domain.DTO;
using Npgsql;


namespace Infrastructure;

public class AccountRepository(IConfiguration config) : IAccountRepository, IDisposable
{
    private readonly NpgsqlConnection _sql = new(config.GetConnectionString("postgresqlConnection"));

    public async Task<CreateAccountWithUserReturn> CreateNewAccountWithUser(string firstName, string lastName, string email, string password)
    {
        Open();
        var accountSql = $"insert into accounts (accountname) VALUES ('test again') RETURNING accountid;";
        var accountId = await _sql.ExecuteScalarAsync<int>(accountSql);
            
        var param = new Dictionary<string, object>
        {
            {"accountid", accountId},
            {"firstname", firstName},
            {"lastname", lastName},
            {"email", email},
            {"hashedpassword", password}
        };
        var sql =
            @"insert into appusers (accountid, firstname, lastname, email, hashedpassword) 
                VALUES (@accountid, @firstname, @lastname, @email, @hashedpassword ) RETURNING userid;";
        var userId = await _sql.ExecuteScalarAsync<int>(sql,param);

        return new CreateAccountWithUserReturn
        {
            UserId = userId,
            AccountId = accountId
        };
    }

    public async Task<AppUser?> GetAppUserByEmail(string email)
    {
        Open();
        var sql = "select accountid, hashedpassword, lastname, email, firstname, userid from appusers where email = @email";
        var param = new Dictionary<string, object>() {{"email", email}};
        return (await _sql.QueryAsync<AppUser>(sql, param)).FirstOrDefault();
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