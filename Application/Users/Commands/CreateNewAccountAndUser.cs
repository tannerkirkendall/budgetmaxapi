using Application.Interfaces;
using MediatR;

namespace Application.Users.Commands;

public class CreateNewAccountAndUserHandler : IRequestHandler<CreateNewAccountAndUserCommand, CreateNewAccountAndUserResult>
{
    public async Task<CreateNewAccountAndUserResult> Handle(CreateNewAccountAndUserCommand request, CancellationToken cancellationToken)
    {
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

}
