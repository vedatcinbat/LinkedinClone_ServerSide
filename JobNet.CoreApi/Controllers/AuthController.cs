using JobNet.CoreApi.Auth;
using JobNet.CoreApi.Services.UserService;
using Microsoft.AspNetCore.Mvc;


namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] UserCredentials credentials)
    {
        var user = await _userService.Authenticate(credentials.Email, credentials.Password);
        
        if (user == null)
            return BadRequest("Invalid email or password.");

        var token = TokenHandler.CreateToken(_configuration, user);

        return Ok(token);
    }
}