using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Linq;
using Vorn.Files.Host.Models;

namespace Vorn.Files.Host.Services;

public class ContentTypeProvider(FileInformationsService fileInformationsService) : IContentTypeProvider
{
    public bool TryGetContentType(string subpath, out string contentType)
    {
        contentType = null;
        if(string.IsNullOrWhiteSpace(subpath))
        {
            return false;
        }
        FileInformation? fileInfo = fileInformationsService.Files.FirstOrDefault(f => f.Url.Contains(subpath, StringComparison.OrdinalIgnoreCase));
        if(fileInfo == null)
        {
            return false;
        }
        contentType = fileInfo.ContentType;
        return true;
    }
}
