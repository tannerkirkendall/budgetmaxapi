
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{

    private readonly ILogger<TestController> _logger;
    private readonly IRepository _repo;

    public TestController(ILogger<TestController> logger, IRepository repo )
    {
        _logger = logger;
        _repo = repo;
    }

    [HttpGet(Name = "GetName")]
    public AppUser Get()
    {

        var user = _repo.GetAppUserByEmail("tannerkirkendall@gmail.com").Result.FirstOrDefault();

        return user ?? new AppUser();
    }
}