using Domain;
using Domain.DTO;

namespace Application.Common.Interfaces;

public interface IAccountRepository
{
    Task<AppUser?> GetAppUserByEmail(string email);

    Task<CreateAccountWithUserReturn> CreateNewAccountWithUser(string firstName, string lastName, string email,
        string password);
}