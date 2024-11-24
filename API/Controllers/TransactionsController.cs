using Application.UseCases.HomePage.Transactions.Commands;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class TransactionsController(ILogger<CategoriesController> logger) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveTransaction([FromBody] SaveNewTransactionCommand command)
    {
        try
        {
            var returnValue = await Mediator.Send(command);
            return Ok(returnValue);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return BadRequest(e);
        }
    }
}