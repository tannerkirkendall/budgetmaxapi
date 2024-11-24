using Application.Common.Helpers;
using Application.Common.Interfaces;
using MediatR;

namespace Application.UseCases.LoginOrSignup.Queries;

public class ValidateEmailAndPasswordHandler: IRequestHandler<ValidateEmailAndPasswordRequest, ValidateEmailAndPasswordResult>
{
    private readonly IAccountRepository _repo;

    public ValidateEmailAndPasswordHandler(IAccountRepository repo)
    {
        _repo = repo;
    }
    
    public async Task<ValidateEmailAndPasswordResult> Handle(ValidateEmailAndPasswordRequest request, CancellationToken cancellationToken)
    {
        var account = (await _repo.GetAppUserByEmail(request.Email));
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