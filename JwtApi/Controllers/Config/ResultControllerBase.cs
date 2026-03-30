using JwtApi.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JwtApi.Controllers.Config;

[ApiController]
public class ResultControllerBase : ControllerBase
{
    protected IActionResult HandleError<T>(Result<T> result)
    {
        if (result.IsSuccess) return Ok(result.Value);
        
        var error = result.Error;

        return Problem(
            title: error.Code,
            detail: error.Message,
            statusCode: error.StatusCode
        );
    }
}