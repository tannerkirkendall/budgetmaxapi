using Application.Interfaces;
using MediatR;

namespace Application.Users.Commands;

public class CreateNewAccountAndUserHandler : IRequestHandler<CreateNewAccountAndUserRequest, CreateNewAccountAndUserResult>
{
    public async Task<CreateNewAccountAndUserResult> Handle(CreateNewAccountAndUserRequest request, CancellationToken cancellationToken)
    {
        return new CreateNewAccountAndUserResult();
    }


}

public class CreateNewAccountAndUserRequest : IRequest<CreateNewAccountAndUserResult>
{

}

public class CreateNewAccountAndUserResult
{

}
