using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace Vorn.Files.Host.Services;

public class HostService(FileInformationsService fileService) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => fileService.Load(cancellationToken);
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
