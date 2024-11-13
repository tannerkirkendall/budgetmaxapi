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

    public CreateAccountWithUserReturn CreateNewAccountWithUser(string firstName, string lastName, string email, string password)
    {
        using (var connection = _sql)
        {
            var accountSql = $"insert into accounts (accountname) VALUES ('test again') RETURNING accountid;";
            var accountId = connection.ExecuteScalar<int>(accountSql);
            
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
            var userId = connection.ExecuteScalar<int>(sql,param);
            
            return new CreateAccountWithUserReturn
            {
                UserId = userId,
                AccountId = accountId
            };

        }
    }

    
    public void Dispose()
    {
    }

     public class CreateAccountWithUserReturn
    {
        public int UserId { get; init; }
        public int AccountId { get; init; }
    }
}
