using Application.Helpers;
using Application.Interfaces;
using MediatR;

namespace Application.Users.Commands;

public class CreateNewAccountAndUserHandler(
    IAccountRepository accountRepository,
    ICategoriesRepository categoriesRepository)
    : IRequestHandler<CreateNewAccountAndUserCommand, CreateNewAccountAndUserResult>
{


    public async Task<CreateNewAccountAndUserResult> Handle(CreateNewAccountAndUserCommand request, CancellationToken cancellationToken)
    {

        var user = await accountRepository.GetAppUserByEmail(request.Email);

        if (user == null)
        {
            var hashedPassword = request.Password.ToHashedBase64String();
            var a = await accountRepository.CreateNewAccountWithUser(request.FirstName, request.LastName,
                request.Email, hashedPassword);
            await SetupCategories(a.AccountId);
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

    private async Task SetupCategories(int accountId)
    {
        var catId = await categoriesRepository.CreateNewCategory(accountId, "Bills");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Gas");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Phone");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Internet");
        
        catId = await categoriesRepository.CreateNewCategory(accountId, "Income");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Paycheck");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Interest");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Free Parking");
        
        catId = await categoriesRepository.CreateNewCategory(accountId, "Home");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Maintenance");
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
