using Application.HomePage.Queries.Transactions;
using Application.HomePage.Transactions.Commands;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class TransactionsController : ApiControllerBase
{
    private readonly ILogger<CategoriesController> _logger;

    public TransactionsController(ILogger<CategoriesController> logger)
    {
        _logger = logger;
    }

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
            _logger.LogError(e.Message);
            return BadRequest(e);
        }
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetTransactions()
    {
        try
        {
            var returnValue = await Mediator.Send(new GetTransactionsRequest());
            return Ok(returnValue);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return BadRequest(e);
        }
    }
    
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(int budgetHeaderId)
    {
        try
        {
            var returnValue = await Mediator.Send(new GetSummaryByBudgetHeaderId.Request(budgetHeaderId));
            return Ok(returnValue);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return BadRequest(e);
        }
    }
}