using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Vorn.Files.Host.Models;
using Vorn.Files.Host.Services;

namespace Vorn.Files.Host.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class FilesController(RepositoryService fileService) : ControllerBase
{
    [HttpPost("upload")]
    [ProducesResponseType(typeof(FileInformation), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileInformation), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromHeader(Name = "Content-Checksum")] string? contentChecksum, [FromHeader(Name = "Content-Owner")] string? owner)
    {
        if(file == null || file.Length == 0)
        {
            return BadRequest("File is missing or empty.");
        }

        // Compute MD5 hash of the uploaded file
        using Stream stream = file.OpenReadStream();
        string checksum = ComputeSHA256Checksum(stream);

        // Compare the computed hash with the one provided in the request header
        if(contentChecksum == null || !contentChecksum.Equals(checksum, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("File integrity check failed. Checksum mismatch.");
        }

        FileInformation? result = await fileService.SaveFile(file, checksum, owner);
        if(result == null)
        {
            return BadRequest();
        }

        return Ok(result);
    }

    static string ComputeSHA256Checksum(Stream fileStream)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(fileStream);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

}