using JwtApi.Controllers.Config;
using JwtApi.DTOs;
using JwtApi.Services;
using JwtApi.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace JwtApi.Controllers;
[ApiController]
public class AuthController : ResultControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UserService _userService;
    private readonly TokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(ILogger<UserController> logger, UserService userService, TokenService tokenService, JwtSettings jwtSettings)
    {
        _logger = logger;
        _userService = userService;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings;
    }
    
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthDto.Register request)
    {
        var result = await _userService.RegisterAsync(request.Username, request.Password);
        return result.IsSuccess ? Ok() : HandleError(result);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthDto.Login request)
    {
        var result = await _userService.LoginAsync(request.Username, request.Password);
        if (!result.IsSuccess) return HandleError(result);

        var tokens = _tokenService.GenerateTokens(result.Value!);

        SetCookies(tokens.AccessToken, tokens.RefreshToken);
        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refresh-token"];
        if (string.IsNullOrEmpty(refreshToken)) return Unauthorized();

        var result = await _tokenService.RefreshToken(refreshToken);
        if (!result.IsSuccess) return HandleError(result);

        var tokens = result.Value!;
        
        SetCookies(tokens.AccessToken, tokens.RefreshToken);

        return Ok();
    }

    /* Helpers ------------- */
    private void SetCookies(string accessToken, string refreshToken)
    {
        Response.Cookies.Append("access-token",  accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessMinutes)
        });
        Response.Cookies.Append("refresh-token",  refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshDays)
        });
        
    }
    
}