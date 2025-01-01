using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace Vorn.File.Host.Services;

public class FileHostedService(FileService fileService) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => fileService.Load(cancellationToken);
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
