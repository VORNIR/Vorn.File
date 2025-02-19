namespace Vorn.Files.Client;

public class FileInformationDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Extension { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public string Md5 { get; set; }
    public string Url { get; set; }
    public DateTime UploadTime { get; set; } = DateTime.Now;
}