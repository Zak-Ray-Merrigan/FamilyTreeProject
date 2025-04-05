using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FamilyTreeLibrary.Infrastructure
{
    public class FamilyTreeContainerLifecycleService(FamilyTreeContainer container, IExtendedLogger<FamilyTreeContainerLifecycleService> logger) : IHostedService
    {
        private readonly FamilyTreeContainer container = container;
        private readonly IExtendedLogger<FamilyTreeContainerLifecycleService> logger = logger;

        public async Task StartAsync(CancellationToken token)
        {
            logger.LogInformation("Starting Neo4j container or determining if it has already been started.");
            await container.Start();
            logger.LogInformation("Neo4j container is running.");
        }

        public async Task StopAsync(CancellationToken token)
        {
            logger.LogInformation("The Neo4j container is being stopped, assuming that it's running.");
            await container.Stop();
            logger.LogInformation("The Neo4j container has already been stopped.");
        }
    }
}