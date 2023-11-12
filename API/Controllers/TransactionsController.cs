﻿using Application.HomePage.Queries.Transactions;
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
}