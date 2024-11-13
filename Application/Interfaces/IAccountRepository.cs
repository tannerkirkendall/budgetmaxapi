using Domain;
using Domain.DTO;

namespace Application.Interfaces;

public interface IAccountRepository
{
    Task<AppUser?> GetAppUserByEmail(string email);

    Task<CreateAccountWithUserReturn> CreateNewAccountWithUser(string firstName, string lastName, string email,
        string password);
}