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

    [HttpGet("all")]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var returnValue = await Mediator.Send(new GetCategoriesAndSubCategoriesRequest());
            return Ok(returnValue);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return BadRequest(e);
        }
    }
}