using FileUploader.Services;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = FileUploader.DTOs.LoginRequest;
using RegisterRequest = FileUploader.DTOs.RegisterRequest;

namespace FileUploader.Controllers;
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UserService _userService;
    
    public AuthController(ILogger<UserController> logger, UserService userService)
    {
        _logger = logger;
        _userService = userService;
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
        var success = await _userService.LoginAsync(request.Username, request.Password);
        return success ? Ok() : BadRequest();
    }
}