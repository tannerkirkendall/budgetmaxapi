
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IRepository _repo;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IRepository repo )
    {
        _logger = logger;
        _repo = repo;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public string Get()
    {

        //var user = _repo.GetAppUserByEmail("tan").Result.ToList();

        return "hello";
    }
}