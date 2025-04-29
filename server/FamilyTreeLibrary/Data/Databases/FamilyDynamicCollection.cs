using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Serialization;
using Microsoft.Azure.Cosmos;

namespace FamilyTreeLibrary.Data.Databases
{
    public class FamilyDynamicCollection : IContainer<FamilyDynamic>
    {
        private readonly string containerName;
        private readonly Container container;

        public FamilyDynamicCollection(FamilyTreeConfiguration configuration, FamilyTreeVault vault)
        {
            containerName = configuration["CosmosDB:FamilyDynamicContainerName"];
            CosmosClient client = new(vault["CosmosDBConnectionString"].AsString, new()
            {
                Serializer = new FamilyTreeDatabaseSerializer(),
                RequestTimeout = TimeSpan.FromMinutes(2),
                ConnectionMode = ConnectionMode.Direct
            });
            Database database = client.GetDatabase(configuration["CosmosDB:FamilyDynamicContainerName"]);
            container = database.GetContainer(containerName);
        }

        public FamilyDynamic this[Guid id]
        {
            get
            {
                string query = $"SELECT * FROM {containerName} f WHERE f.id = @id";
                QueryDefinition queryDefinition = new QueryDefinition(query)
                    .WithParameter("@id", id.ToString());
                using FeedIterator<FamilyDynamic> feed = container.GetItemQueryIterator<FamilyDynamic>(queryDefinition);
                FeedResponse<FamilyDynamic> response = feed.ReadNextAsync().Result;
                IEnumerable<FamilyDynamic> familyDynamics = response.Resource;
                return familyDynamics.First();
            }
            set
            {
                FindFamilyDynamic(value, out Guid actualId);
                if (value.Id != actualId)
                {
                    FamilyDynamic familyDynamic = new(new Dictionary<string, BridgeInstance>(value.Instance.AsObject)
                    {
                        ["id"] = new(actualId.ToString())
                    });
                    container.UpsertItemAsync(familyDynamic, new PartitionKey(value.FamilyDynamicStartDate.ToString())).Wait();
                }
                else
                {
                    container.UpsertItemAsync(value, new PartitionKey(value.FamilyDynamicStartDate.ToString())).Wait();
                }
            }
        }

        public void Remove(FamilyDynamic familyDynamic)
        {
            container.DeleteItemAsync<FamilyDynamic>(familyDynamic.Id.ToString(), new PartitionKey(familyDynamic.FamilyDynamicStartDate.ToString())).Wait();
        }

        public void UpdateOrCreate(FamilyDynamic familyDynamic)
        {
            FindFamilyDynamic(familyDynamic, out Guid actualId);
            if (familyDynamic.Id != actualId)
            {
                FamilyDynamic familyDynamic1 = new(new Dictionary<string, BridgeInstance>(familyDynamic.Instance.AsObject)
                {
                    ["id"] = new(actualId.ToString())
                });
                container.UpsertItemAsync(familyDynamic1, new PartitionKey(familyDynamic.FamilyDynamicStartDate.ToString())).Wait();
            }
            else
            {
                container.UpsertItemAsync(familyDynamic, new PartitionKey(familyDynamic.FamilyDynamicStartDate.ToString())).Wait();
            }
        }

        private void FindFamilyDynamic(FamilyDynamic familyDynamic, out Guid id)
        {
            string query = $"SELECT VALUE f.id FROM {containerName} f WHERE f.familyDynamicStartDate = @familyDynamicStartDate AND f.pageTitle = @pageTitle";
            QueryDefinition definition = new QueryDefinition(query)
                .WithParameter("@familyDynamicStartDate", familyDynamic.FamilyDynamicStartDate)
                .WithParameter("@pageTitle", new Bridge(familyDynamic.PageTitle));
            using FeedIterator<FamilyDynamic> feed = container.GetItemQueryIterator<FamilyDynamic>(definition);
            if (feed.HasMoreResults)
            {
                FeedResponse<FamilyDynamic> response = feed.ReadNextAsync().Result;
                IEnumerable<FamilyDynamic> results  = response.Resource;
                int count = results.Count();
                if (count > 1)
                {
                    throw new UniquenessViolationException("There can't be duplicate family dynamics.");
                }
                else if (count == 1)
                {
                    id = results.First().Id;
                }
                else
                {
                    id = familyDynamic.Id;
                }
            }
            else
            {
                id = familyDynamic.Id;
            }
        }
    }
}