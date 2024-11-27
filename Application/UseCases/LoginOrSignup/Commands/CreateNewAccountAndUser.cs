using Application.Common.Helpers;
using Application.Common.Interfaces;
using MediatR;

namespace Application.UseCases.LoginOrSignup.Commands;

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
        var catId = await categoriesRepository.CreateNewCategory(accountId, "Essentials");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Groceries");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Medical");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Piper");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Clothes");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Gas");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Home Maintenance");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Babysitting");

        catId = await categoriesRepository.CreateNewCategory(accountId, "Bills");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Tithe");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Mortgage");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Health Insurance");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Internet/Electric");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Cellphones");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Car Insurance");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Water");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Natural Gas");
        
        catId = await categoriesRepository.CreateNewCategory(accountId, "Income");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Paycheck");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Interest");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Free Parking");
        
        catId = await categoriesRepository.CreateNewCategory(accountId, "Non-essentials");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Gift");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Eat Out");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Home Merchandise");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Amazon");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Misc Spending");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Extra Giving");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Subscription Misc");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Date Night");
        await categoriesRepository.CreateNewSubCategory(accountId, catId, "Entertainment");
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
