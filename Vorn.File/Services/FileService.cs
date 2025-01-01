using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Vorn.File.Host.Models.Data;
using Vorn.File.Host.Options;

namespace Vorn.File.Host.Services;

public class FileService(IOptions<VornOptions> options)
{
    public List<FileInformation> FilesList { get; set; } = [];
    public async Task Save()
    {
        XmlSerializer serializer = new(typeof(List<FileInformation>));
        using MemoryStream memory = new();
        using XmlTextWriter xmlWriter = new(memory, Encoding.UTF8);
        xmlWriter.Formatting = Formatting.Indented;
        serializer.Serialize(xmlWriter, FilesList);
        string xml = Encoding.UTF8.GetString(memory.ToArray());
        await System.IO.File.WriteAllTextAsync(options.Value.File.FileInformationsFile, xml);
        await Load();
    }
    public async Task Load(CancellationToken cancellationToken = default)
    {
        string file = options.Value.File.FileInformationsFile;
        if(!System.IO.File.Exists(file))
        {
            return;
        }
        string xml = await System.IO.File.ReadAllTextAsync(file, cancellationToken);
        XmlSerializer serializer = new(typeof(List<FileInformation>));
        using Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        FilesList = serializer.Deserialize(stream) as List<FileInformation> ?? [];
    }
}
