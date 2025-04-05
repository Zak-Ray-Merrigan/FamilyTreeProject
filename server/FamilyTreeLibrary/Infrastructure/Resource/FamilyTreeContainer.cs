using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ContainerInstance;
using Azure.ResourceManager.Resources;

namespace FamilyTreeLibrary.Infrastructure.Resource
{
    public class FamilyTreeContainer
    {
        private readonly FamilyTreeConfiguration configuration;
        private readonly ArmClient client;
        private readonly ContainerGroupResource resource;

        public FamilyTreeContainer(FamilyTreeConfiguration configuration)
        {
            this.configuration = configuration;
            client = new(new DefaultAzureCredential());
            resource = GetContainerResource();
        }

        public async Task Start()
        {
            if (resource.Data.InstanceView.State != "Running")
            {
                await resource.StartAsync(WaitUntil.Completed);
            }
        }

        public async Task Stop()
        {
            if (resource.Data.InstanceView.State == "Running")
            {
                await resource.StopAsync();
            }
        }

        private ContainerGroupResource GetContainerResource()
        {
            SubscriptionResource subscription = client.GetSubscriptionResource(new ResourceIdentifier(configuration["SubscriptionId"]));
            ResourceGroupResource resourceGroup = subscription.GetResourceGroup(configuration["ResourceGroupName"]).Value;
            return resourceGroup.GetContainerGroup(configuration["FamilyTreeContainer:Name"]).Value;
        }
    }
}