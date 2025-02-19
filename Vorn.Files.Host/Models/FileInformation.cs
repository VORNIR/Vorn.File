using System;

namespace Vorn.Files.Host.Models;

public class FileInformation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Owner { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public string Checksum { get; set; }
    public string Url { get; set; }
    public DateTime UploadTime { get; set; } = DateTime.Now;
}