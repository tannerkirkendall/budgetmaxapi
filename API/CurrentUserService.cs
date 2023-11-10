using Application.Interfaces;

namespace API;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _context;

    public CurrentUserService(IHttpContextAccessor context)
    {
        _context = context;
    }

    public int AccountId
    {
        get
        {
            var val = _context.HttpContext?.User.FindFirst("accountId")?.Value ?? "0";
            return int.Parse(val);
        }
    }
    
    public int UserId
    {
        get
        {
            var val = _context.HttpContext?.User.FindFirst("userId")?.Value ?? "0";
            return int.Parse(val);
        }
    }
}