using Domain.Interfaces;

namespace Domain;

public class AppUser : IEntity
{
    public int UserId { get; init; }
    public int AccountId { get; init; }
    public string FirstName { get; init; } = "";
    public string HashedPassword { get; init; } = "";
    public string Email { get; init; } = "";
}