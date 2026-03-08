using FileUploader.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileUploader.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UserService _userService;
    
}