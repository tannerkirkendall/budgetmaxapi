using Application.HomePage.Queries.Categories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ApiControllerBase
{

    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ILogger<CategoriesController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetCategories")]
    public async Task<IActionResult> GetCategories(int accountId)
    {
        try
        {
            var returnValue = await Mediator.Send(new GetCategoriesAndSubCategoriesRequest(accountId));
            return Ok(returnValue);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return BadRequest(e);
        }
    }
}