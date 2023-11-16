using Application.Helpers;
using Application.Interfaces;
using MediatR;

namespace Application.Authentication.Queries;

public class ValidateEmailAndPasswordHandler: IRequestHandler<ValidateEmailAndPasswordRequest, ValidateEmailAndPasswordResult>
{
    private readonly IRepository _repo;

    public ValidateEmailAndPasswordHandler(IRepository repo)
    {
        _repo = repo;
    }
    
    public async Task<ValidateEmailAndPasswordResult> Handle(ValidateEmailAndPasswordRequest request, CancellationToken cancellationToken)
    {
        var account = (await _repo.GetAppUserByEmail(request.Email)).SingleOrDefault();
        if (account == null)
        {
            return new ValidateEmailAndPasswordResult(false,0 ,0);
        }

        var hashedPassword = request.Password.ToHashedBase64String();
        if (account.HashedPassword == hashedPassword)
        {
            return new ValidateEmailAndPasswordResult(true, account.AccountId, account.UserId);
        }
        
        return new ValidateEmailAndPasswordResult(false,0 ,0);
    }
}

public class ValidateEmailAndPasswordRequest : IRequest<ValidateEmailAndPasswordResult>
{
    public ValidateEmailAndPasswordRequest(string email, string password)
    {
        Email = email;
        Password = password;
    }
    public string Email { get; }
    public string Password { get; }
}

public class ValidateEmailAndPasswordResult
{
    public ValidateEmailAndPasswordResult(bool authenticated, int accountId, int userId)
    {
        Authenticated = authenticated;
        AccountId = accountId;
        UserId = userId;
    }
    public bool Authenticated { get; }
    public int AccountId { get; }
    public int UserId { get; }
}