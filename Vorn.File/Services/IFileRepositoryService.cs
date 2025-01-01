using Vorn.File.Host.Models.Data;
using Vorn.File.Host.Models.Local;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Vorn.File.Host.Services;

public interface IFileRepositoryService
{
    Task<Result<FileInformation>> UploadFileAsync(IFormFile formFile);
}