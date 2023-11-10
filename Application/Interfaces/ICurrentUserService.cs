namespace Application.Interfaces;

public interface ICurrentUserService
{
    public int AccountId { get; }
    public int UserId { get; }
}