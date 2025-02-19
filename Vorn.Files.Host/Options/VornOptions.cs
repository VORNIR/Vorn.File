namespace Vorn.Files.Host.Options;

public class VornOptions
{
    public const string Section = nameof(Vorn);
    public FilesOptions Files { get; set; }
    public AaasOptions Aaas { get; set; }
}
