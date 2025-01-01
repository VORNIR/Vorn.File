using Vorn.File.Host.Models.Data;
using Vorn.File.Host.Models.Local;
using Vorn.File.Host.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Vorn.File.Host.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class FilesController(IFileRepositoryService fileService) : ControllerBase
{
    private readonly IFileRepositoryService _fileService = fileService;

	[HttpPost("upload")]
    [ProducesResponseType(typeof(Result<FileInformation>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<FileInformation>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFileAsync(IFormFile file)
    {
        var result = await _fileService.UploadFileAsync(file);

        if (!result.Success) return BadRequest(result);

        return Ok(result);
    }
}