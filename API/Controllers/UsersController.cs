using Application.UseCases.Users.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ApiControllerBase
{
    private readonly ILogger<CategoriesController> _logger;

    public UsersController(ILogger<CategoriesController> logger)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("CreateNewAccount")]
    public async Task<IActionResult> PostCreateNewAccount([FromBody] CreateNewAccountAndUserCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.AccountAlreadyExists)
        {
            return BadRequest("Account Already Exists");
        }
        return Ok();
    }
}