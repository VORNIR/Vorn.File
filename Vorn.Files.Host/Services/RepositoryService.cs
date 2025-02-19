using HeyRed.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vorn.Files.Host.Models;
using Vorn.Files.Host.Options;

namespace Vorn.Files.Host.Services;

public class RepositoryService(IOptions<VornOptions> options, FileInformationsService fileInfoService)
{
    public async Task<FileInformation?> SaveFile(IFormFile file, string checksum, string owner)
    {
        FileInformation? sameChecksum = fileInfoService.Files.SingleOrDefault(f => f.Checksum == checksum);
        if(sameChecksum is not null)
        {
            return sameChecksum;
        }

        FileInformation fileInformation = new();
        string baseServerPath = Path.Combine(Directory.GetCurrentDirectory(), options.Value.Files.UploadPath);
        string extension = MimeTypesMap.GetExtension(file.ContentType);
        string newFileName = $"{options.Value.Files.Prefix}-{fileInformation.Id}.{extension}";
        string serverPath = Path.Combine(baseServerPath, newFileName);

        // Write to disk
        await using FileStream fileStream = new(serverPath, FileMode.Create);
        await file.CopyToAsync(fileStream);

        // Populate file info
        fileInformation.Owner = owner;
        fileInformation.Checksum = checksum;
        fileInformation.Size = file.Length;
        fileInformation.ContentType = file.ContentType;
        fileInformation.Url = new Uri(new Uri(options.Value.Files.BaseUrl), Path.Combine(options.Value.Files.UploadPath, newFileName)).ToString();

        // Save file information
        await fileInfoService.Add(fileInformation);
        return fileInformation;
    }

    public async Task<bool> DeleteFile(FileInformation fileInformation)
    {
        FileInformation? file = fileInfoService.Files.SingleOrDefault(x => x.Id == fileInformation.Id && x.Checksum == fileInformation.Checksum);
        if(file == null)
        {
            return false;
        }
        await fileInfoService.Remove(file);
        return true;
    }
}