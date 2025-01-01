using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Vorn.File.Host.Models.Data;
using Vorn.File.Host.Models.Local;
using Vorn.File.Host.Models.Repository;
using Vorn.File.Host.Options;

namespace Vorn.File.Host.Services;

public class FileRepositoryService(IOptions<VornOptions> options, IFileRepository fileRepository) : IFileRepositoryService
{

    public async Task<Result<FileInformation>> UploadFileAsync(IFormFile file)
    {
        FileInformation fileInformation = new();
        string baseServerPath = Path.Combine(Directory.GetCurrentDirectory(), options.Value.File.UploadPath);
        string extension = Path.GetExtension(file.FileName);
        string newFileName = string.Concat($"VORN.FILE-{fileInformation.Id}", extension);
        string serverPath = Path.Combine(baseServerPath, newFileName);
        using MD5 md5 = MD5.Create();
        using FileStream filestream = new FileStream(serverPath, FileMode.Create);
        await file.CopyToAsync(filestream);
        byte[] md5bytes = md5.ComputeHash(filestream);
        string md5str = Convert.ToBase64String(md5bytes, 0, md5bytes.Length);
        fileInformation.Md5 = md5str;
        fileInformation.Extension = extension;
        fileInformation.Size = file.Length;
        fileInformation.ContentType = file.ContentType;
        fileInformation.Url = new Uri(new Uri(options.Value.File.BaseUrl), Path.Combine(options.Value.File.UploadPath, newFileName)).ToString();
        Result<FileInformation> result = await fileRepository.AddAsync(fileInformation);
        return result;
    }
}