using FileUploader.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FileUploader.Controllers.Config;

[ApiController]
public class ResultControllerBase : ControllerBase
{
    protected IActionResult ProcessResult<T>(Result<T> result)
    {
        if (result.IsSuccess) return Ok(result.Value);
        
        return StatusCode(result.Error.StatusCode, result.Error);
    }
}