using Application.UseCases.HomePage.Categories.Queries;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ILogger<CategoriesController> logger) : ApiControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var returnValue = await Mediator.Send(new GetCategoriesAndSubCategoriesQuery());
            return Ok(returnValue);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return BadRequest(e);
        }
    }
}