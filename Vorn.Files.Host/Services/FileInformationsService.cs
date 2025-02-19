using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Vorn.Files.Host.Models;
using Vorn.Files.Host.Options;

namespace Vorn.Files.Host.Services;

public class FileInformationsService(IOptions<VornOptions> options)
{
    private readonly SemaphoreSlim semaphore = new(1);
    List<FileInformation> FilesList { get; set; } = [];
    public IEnumerable<FileInformation> Files => FilesList;
    async Task Save()
    {
        await semaphore.WaitAsync();
        XmlSerializer serializer = new(typeof(List<FileInformation>));
        using MemoryStream memory = new();
        using XmlTextWriter xmlWriter = new(memory, Encoding.UTF8);
        xmlWriter.Formatting = Formatting.Indented;
        serializer.Serialize(xmlWriter, FilesList);
        string xml = Encoding.UTF8.GetString(memory.ToArray());
        await File.WriteAllTextAsync(options.Value.Files.FileInformationsFile, xml);
        semaphore.Release();
    }
    public async Task Add(FileInformation fileInformation)
    {
        FilesList.Add(fileInformation);
        await Save();
    }
    public async Task Remove(FileInformation fileInformation)
    {
        FilesList.Remove(fileInformation);
        await Save();
    }
    public async Task Load(CancellationToken cancellationToken = default)
    {
        string file = options.Value.Files.FileInformationsFile;
        if(!File.Exists(file))
        {
            return;
        }
        string xml = await File.ReadAllTextAsync(file, cancellationToken);
        XmlSerializer serializer = new(typeof(List<FileInformation>));
        using Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        FilesList = serializer.Deserialize(stream) as List<FileInformation> ?? [];
    }
}
