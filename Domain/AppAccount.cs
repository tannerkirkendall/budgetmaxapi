using Domain.Interfaces;

namespace Domain;

public class AppAccount : IEntity
{
    public int AccountId { get; init; }
    public string AccountName { get; init; }
}