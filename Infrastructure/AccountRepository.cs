using System.Security.Cryptography;
using System.Text;
using Dapper;
using Microsoft.Extensions.Configuration;
using Application.Interfaces;
using Npgsql;

namespace Infrastructure;

public class AccountRepository : IAccountRepository, IDisposable
{
    private readonly NpgsqlConnection _sql;

    public AccountRepository(IConfiguration config)
    {
        var a = new NpgsqlConnection();
        a.ConnectionString = config.GetConnectionString("postgresqlConnection");
        _sql = a;
    }

    public void CreateNewAccountWithUser()
    {
        using (var connection = _sql)
        {
            var password = Convert.ToBase64String(SHA256.HashData(Encoding.ASCII.GetBytes("plainText")));
            var accountId = connection.ExecuteScalar<int>("insert into accounts (accountname) VALUES ('test again') RETURNING accountid;");
            var param = new Dictionary<string, object>
            {
                {"accountid", accountId},
                {"firstname", "tanner"},
                {"lastname", "kirk"},
                {"email", "tanner@gmail.com"},
                {"hashedpassword", password}
            };
            var sql =
                @"insert into appusers (accountid, firstname, lastname, email, hashedpassword) 
                VALUES (@accountid, @firstname, @lastname, @email, @hashedpassword ) RETURNING userid;";
            var userId = connection.ExecuteScalar<int>(sql,param);
            
            
        }
    }
    
    public void Dispose()
    {
    }
}