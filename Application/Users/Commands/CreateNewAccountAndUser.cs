using Application.Helpers;
using Application.Interfaces;
using MediatR;

namespace Application.Users.Commands;

public class CreateNewAccountAndUserHandler : IRequestHandler<CreateNewAccountAndUserCommand, CreateNewAccountAndUserResult>
{
    private readonly IAccountRepository _accountRepository;

    public CreateNewAccountAndUserHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
    
    public async Task<CreateNewAccountAndUserResult> Handle(CreateNewAccountAndUserCommand request, CancellationToken cancellationToken)
    {

        var user = await _accountRepository.GetAppUserByEmail(request.Email);

        if (user == null)
        {
            var hashedPassword = request.Password.ToHashedBase64String();
            var a = await _accountRepository.CreateNewAccountWithUser(request.FirstName, request.LastName,
                request.Email, hashedPassword);
        }
        else
        {
            return new CreateNewAccountAndUserResult
            {
                AccountAlreadyExists = true
            };
        }


        return new CreateNewAccountAndUserResult();
    }


}

public class CreateNewAccountAndUserCommand : IRequest<CreateNewAccountAndUserResult>
{
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Password { get; init; } = "";
    public string Email { get; init; } = "";

}

public class CreateNewAccountAndUserResult
{
    public bool AccountAlreadyExists { get; init; }

}
