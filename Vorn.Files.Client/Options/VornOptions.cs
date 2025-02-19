namespace Vorn.Files.Client.Options;

public class VornOptions
{
    public const string Section = nameof(Vorn);
    public VornFilesClientOptions Files { get; set; }
}
