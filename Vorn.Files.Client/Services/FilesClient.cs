using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using Vorn.Files.Client.Options;

namespace Vorn.Files.Client;

public class FilesClient(HttpClient httpClient, IOptions<VornOptions> options)
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<FileInformationDto?> UploadFile(Stream fileStream, string contentType, CancellationToken cancellationToken = default)
    {
        // Compute MD5 hash of the file stream
        string checksum = ComputeSHA256Checksum(fileStream);

        // Reset the fileStream position to ensure it's read from the beginning
        fileStream.Position = 0;

        using MultipartFormDataContent content = new MultipartFormDataContent();
        using StreamContent fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        // Add the file content
        content.Add(fileContent, "file", "fileName");

        content.Headers.Add("Content-Checksum", checksum);
        content.Headers.Add("Content-Owner", options.Value.Files.Owner);

        HttpResponseMessage response = await httpClient.PostAsync($"{options.Value.Files.Host}/api/v1/Files/upload", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        FileInformationDto? fileInformation = JsonSerializer.Deserialize<FileInformationDto>(responseBody, jsonSerializerOptions);

        return fileInformation;
    }

    static string ComputeSHA256Checksum(Stream fileStream)
    {
        using var sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(fileStream);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

}
