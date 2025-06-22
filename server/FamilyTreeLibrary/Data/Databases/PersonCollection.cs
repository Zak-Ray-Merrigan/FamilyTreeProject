using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Serialization;
using Microsoft.Azure.Cosmos;

namespace FamilyTreeLibrary.Data.Databases
{
    public class PersonCollection : IContainer<Person>
    {
        private readonly Container container;
        private readonly string containerName;

        public PersonCollection(FamilyTreeConfiguration configuration, FamilyTreeVault vault)
        {
            CosmosClient client = new(vault["CosmosDBConnectionString"].AsString, new CosmosClientOptions()
            {
                Serializer = new FamilyTreeDatabaseSerializer(),
                RequestTimeout = TimeSpan.FromMinutes(2),
                ConnectionMode = ConnectionMode.Direct
            });
            containerName = configuration["CosmosDB:PersonContainerName"];
            Database database = client.GetDatabase(configuration["CosmosDB:DatabaseName"]);
            container = database.GetContainer(containerName);
        }

        public Person this[Guid id]
        {
            get
            {
                string query = $"SELECT * FROM {containerName} p WHERE p.id = @id";
                QueryDefinition definition = new QueryDefinition(query)
                    .WithParameter("@id", new Bridge(id.ToString()));
                using FeedIterator<Person> feed = container.GetItemQueryIterator<Person>(definition);
                FeedResponse<Person> response = feed.ReadNextAsync().Result;
                return response.Resource.First();
            }
        }

        public Person this[string birthName, FamilyTreeDate? birthDate = null, FamilyTreeDate? deceasedDate = null]
        {
            get
            {
                string query = $"SELECT VALUE p.id FROM {containerName} p WHERE p.birthName = @birthName AND p.birthDate = @birthDate AND p.deceasedDate = @deceasedDate";
                QueryDefinition definition = new QueryDefinition(query)
                    .WithParameter("@birthName", new Bridge(birthName))
                    .WithParameter("@birthDate", birthDate is null ? new Bridge() : birthDate)
                    .WithParameter("@deceasedDate", deceasedDate is null ? new Bridge() : deceasedDate);
                using FeedIterator<Person> feed = container.GetItemQueryIterator<Person>(definition);
                FeedResponse<Person> response = feed.ReadNextAsync().Result;
                IEnumerable<Person> results  = response.Resource;
                return results.First();
            }
        }

        public void Remove(Person person)
        {
            container.DeleteItemAsync<Person>(person.Id.ToString(), new PartitionKey(person.BirthName)).Wait();
        }

        public void UpdateOrCreate(Person p)
        {
            FindPerson(p, out Guid actualId);
            if (p.Id != actualId)
            {
                IDictionary<string,BridgeInstance> obj = new Dictionary<string,BridgeInstance>(p.Instance.AsObject)
                {
                    ["id"] = new(actualId.ToString())
                };
                Person p1 = new(obj);
                container.UpsertItemAsync(p1, new PartitionKey(p.BirthName)).Wait();
            }
            else
            {
                container.UpsertItemAsync(p, new PartitionKey(p.BirthName)).Wait();
            }
        }

        private void FindPerson(Person person, out Guid id)
        {
            string query = $"SELECT VALUE p.id FROM {containerName} p WHERE p.birthName = @birthName AND p.birthDate = @birthDate AND p.deceasedDate = @deceasedDate";
            QueryDefinition definition = new QueryDefinition(query)
                .WithParameter("@birthName", new Bridge(person.BirthName))
                .WithParameter("@birthDate", person.BirthDate is null ? new Bridge() : person.BirthDate)
                .WithParameter("@deceasedDate", person.DeceasedDate is null ? new Bridge() : person.DeceasedDate);
            using FeedIterator<Person> feed = container.GetItemQueryIterator<Person>(definition);
            if (feed.HasMoreResults)
            {
                FeedResponse<Person> response = feed.ReadNextAsync().Result;
                IEnumerable<Person> results  = response.Resource;
                int count = results.Count();
                if (count > 1)
                {
                    throw new UniquenessViolationException("There can't be duplicate people.");
                }
                else if (count == 1)
                {
                    id = results.First().Id;
                }
                else
                {
                    id = person.Id;
                }
            }
            else
            {
                id = person.Id;
            }
        }
    }
}