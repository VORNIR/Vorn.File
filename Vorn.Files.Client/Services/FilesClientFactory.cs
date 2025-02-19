using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;
using Vorn.Files.Client.Options;

namespace Vorn.Files.Client;

public class FilesClientFactory(IOptions<VornOptions> options)
{
    public async Task<FilesClient> CreateClientAsync()
    {
        HttpClient client = new();
        DiscoveryDocumentResponse discoveryDocument = await client.GetDiscoveryDocumentAsync(options.Value.Files.Authority);
        if(discoveryDocument.IsError)
        {
            throw new Exception($"Error retrieving discovery document: {discoveryDocument.Error}");
        }
        TokenResponse tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = discoveryDocument.TokenEndpoint,
            ClientId = "Vorn.Files",
            ClientSecret = options.Value.Files.Secret,
            Scope = "files"
        });

        if(tokenResponse.IsError || tokenResponse.AccessToken is null)
        {
            throw new Exception($"Error retrieving token: {tokenResponse.Error}");
        }

        HttpClient httpClient = new()
        {
            BaseAddress = new Uri(options.Value.Files.Host)
        };
        httpClient.SetBearerToken(tokenResponse.AccessToken);

        return new FilesClient(httpClient, options);
    }
}
