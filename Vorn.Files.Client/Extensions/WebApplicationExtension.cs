using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vorn.Files.Client.Options;

namespace Vorn.Files.Client;

public static class WebApplicationExtension
{
    public static WebApplicationBuilder AddFilesClient(this WebApplicationBuilder builder)
    {
        VornOptions vornOptions = new();
        IConfigurationSection section = builder.Configuration.GetSection(VornOptions.Section);
        section.Bind(vornOptions);
        builder.Services.Configure<VornOptions>(section);
        builder.Services.AddSingleton<FilesClientFactory>();
        return builder;
    }
}
