using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using Vorn.Files.Client.Options;

namespace Vorn.Files.Client;

public class FilesClient(HttpClient httpClient, IOptions<VornOptions> options)
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<FileInformationDto?> UploadFile(Stream memoryStream, string contentType, CancellationToken cancellationToken = default)
    {
        string checksum = await ComputeSHA256Checksum(memoryStream, cancellationToken);
        using MultipartFormDataContent content = [];
        using StreamContent fileContent = new(memoryStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        content.Add(fileContent, "file", "fileName");

        content.Headers.Add("Content-Checksum", checksum);
        content.Headers.Add("Content-Owner", options.Value.Files.Owner);

        HttpResponseMessage response = await httpClient.PostAsync($"{options.Value.Files.Host}/api/v1/Files/upload", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        FileInformationDto? fileInformation = JsonSerializer.Deserialize<FileInformationDto>(responseBody, jsonSerializerOptions);

        return fileInformation;
    }
    public async Task<FileInformationDto?> UploadFile(byte[] data, string contentType, CancellationToken cancellationToken = default) => await UploadFile(new MemoryStream(data), contentType, cancellationToken);
    static async Task<string> ComputeSHA256Checksum(Stream stream, CancellationToken cancellationToken = default)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = await sha256.ComputeHashAsync(stream, cancellationToken);
        stream.Position = 0;
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

}
