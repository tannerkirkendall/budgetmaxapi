using Application.UseCases.HomePage.Budgets.Commands;
using Application.UseCases.HomePage.Budgets.Queries;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetsController (ILogger<CategoriesController> logger) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddBudget([FromBody] AddNewBudgetCommand command)
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
    
    [HttpGet]
    public async Task<IActionResult> GetBudgets()
    {
        try
        {
            var returnValue = await Mediator.Send(new GetBudgetsQuery());
            return Ok(returnValue);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return BadRequest(e);
        }
    }
    
    [HttpPost("detail")]
    public async Task<IActionResult> AddBudgetDetail([FromBody] AddNewBudgetDetailCommand command)
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
    
    [HttpGet("detail/{budgetId}")]
    public async Task<IActionResult> GetBudgetDetails([FromRoute] int budgetId)
    {
        try
        {
            var returnValue = await Mediator.Send(new GetBudgetDetailQuery(budgetId));
            return Ok(returnValue);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return BadRequest(e);
        }
    }
}