using System.Security.Claims;
using System.Security.Cryptography;
using Application.Authentication.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    private readonly ILogger<CategoriesController> _logger;
    private readonly string _apiKey;

    public AuthController(ILogger<CategoriesController> logger, IConfiguration conf)
    {
        _logger = logger;
        _apiKey = conf.GetSection("PrivateKey")?.Value ?? "";
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> PostLoginForEmployee(string email, string password)
    {
        var returnValue = await Mediator.Send(new ValidateEmailAndPasswordRequest(email, password));
        if (returnValue.Authenticated == false)
        {
            return Unauthorized();
        }
        
        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("email", email),
                new Claim("accountId", returnValue.AccountId.ToString()),
                new Claim("userId", returnValue.UserId.ToString())
            }),
            SigningCredentials = new SigningCredentials(CreateRsaSecurityKey(_apiKey), SecurityAlgorithms.RsaSha256)
        });
        return Ok(token);
    }
    
    [AllowAnonymous]
    [HttpGet("createKey")]
    public string CreateKey()
    {
        var rsaKey = RSA.Create();
        var privateKey = rsaKey.ExportRSAPrivateKey();
        return Convert.ToBase64String(privateKey);
    }
    
    private static RsaSecurityKey CreateRsaSecurityKey(string loginKey)
    {
        var encoded = Convert.FromBase64String(loginKey);
        var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(encoded, out _);
        return new RsaSecurityKey(rsa);
    }
}