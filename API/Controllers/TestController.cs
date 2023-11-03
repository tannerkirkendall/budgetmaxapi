
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
    public string Get()
    {

        var user = _repo.GetAppUserByEmail("tan").Result.FirstOrDefault();

        return user?.Email ?? "No Email";
    }
}