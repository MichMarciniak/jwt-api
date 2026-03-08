using FileUploader.Controllers.Config;
using FileUploader.Services;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = FileUploader.DTOs.LoginRequest;
using RegisterRequest = FileUploader.DTOs.RegisterRequest;

namespace FileUploader.Controllers;
[ApiController]
public class AuthController : ResultControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UserService _userService;
    private readonly AuthService _authService;
    
    public AuthController(ILogger<UserController> logger, UserService userService, AuthService authService)
    {
        _logger = logger;
        _userService = userService;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var success = await _userService.RegisterAsync(request.Username, request.Password);
        return success ? Ok() : BadRequest();
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginRequest request)
    {
        var result = await _userService.LoginAsync(request.Username, request.Password);

        if (!result.IsSuccess)
        {
            return HandleError(result);
        }
        
        var user = result.Value!; // ! to force not null
        
        var accessToken = _authService.GenerateAccessToken(user);

        return Ok(accessToken);
    }
}