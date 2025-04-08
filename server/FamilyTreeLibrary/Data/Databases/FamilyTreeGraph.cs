using FamilyTreeLibrary.Data.Models;
using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Serialization;
using Neo4j.Driver;

namespace FamilyTreeLibrary.Data.Databases
{
    public class FamilyTreeGraph
    {
        private readonly IDriver driver;

        public FamilyTreeGraph(FamilyTreeConfiguration configuration, FamilyTreeVault vault)
        {
            FamilyTreeGraphCredentials credentials = new(vault);
            driver = GraphDatabase.Driver(configuration["FamilyTreeContainers:Uri"], AuthTokens.Basic(credentials.Username, credentials.Password));
        }

        public FamilyTreeNode this[Guid id]
        {
            get
            {
                string query = "MATCH (n:FamilyTreeNode {id: $id}) RETURN n;";
                INode node = Task.Run(async () =>
                {
                    IAsyncSession session = driver.AsyncSession();
                    try
                    {
                        IResultCursor result = await session.RunAsync(query, new {id = id.ToString()});
                        IRecord record = await result.SingleAsync();
                        return record["n"].As<INode>();
                    }
                    finally
                    {
                        await session.CloseAsync();
                    }
                }).Result;
                return ToFamilyTreeNode(node);
            }
        }

        public void AddParentChildRelationship(FamilyTreeNode parent, FamilyTreeNode child)
        {
            string query = @"
                MATCH (parent:FamilyTreeNode {id: $parentId})
                MATCH (child:FamilyTreeNode {id: $childId})
                MERGE (parent)-[:PARENT_OF]->(child)
                MERGE (child)-[:CHILD_OF]->(parent);";
            IAsyncSession session = driver.AsyncSession();
            Task.Run(async() =>
            {
                await session.RunAsync(query, new Dictionary<string,object>()
                {
                    ["parentId"] = parent.Id.ToString(),
                    ["childId"] = child.Id.ToString()
                });
            }).Wait();
            session.CloseAsync().Wait();
        }

        public bool Contains(FamilyTreeNode node)
        {
            string query = @"
                MATCH (node:FamilyTreeNode {id: $id})
                RETURN node;";
            IAsyncSession session = driver.AsyncSession();
            try
            {
                return Task.Run(async() =>
                {
                    IResultCursor cursor = await session.RunAsync(query, new {id = node.Id.ToString()});
                    IList<IRecord> records = await cursor.ToListAsync();
                    return records.Count > 0;
                }).Result;
            }
            finally
            {
                session.CloseAsync().Wait();
            }
        }

        public void CreateNode(FamilyTreeNode node)
        {
            string query = @"
                CREATE (n: FamilyTreeNode {
                    id: $id,
                    inheritedFamilyNames: $inheritedFamilyNames,
                    memberId: $memberId,
                    inLawId: $inLawId,
                    dynamicId: $dynamicId
                });";
            IAsyncSession session = driver.AsyncSession();
            Task.Run(async () =>
            {
                await session.RunAsync(query, new
                {
                    id = node.Id.ToString(),
                    inheritedFamilyNames = node.InheritedFamilyNames,
                    memberId = node.MemberId.ToString(),
                    inLawId = node.InLawId?.ToString() ?? null,
                    dynamicId = node.DynamicId?.ToString() ?? null
                });
            }).Wait();
            session.CloseAsync().Wait();
        }

        public IEnumerable<FamilyTreeNode> GetChildren(FamilyTreeNode node)
        {
            string query = @"
                MATCH (parent:FamilyTreeNode {id: $id})-[:PARENT_OF]->(child:FamilyTreeNode)
                RETURN child;";
            IAsyncSession session = driver.AsyncSession();
            try
            {
                return Task.Run(async() =>
                {
                    IResultCursor cursor = await session.RunAsync(query, new {id = node.Id.ToString()});
                    IList<IRecord> records = await cursor.ToListAsync();
                    return records.Select((record) =>
                    {
                        INode node = record["child"].As<INode>();
                        return ToFamilyTreeNode(node);
                    });
                }).Result;
            }
            finally
            {
                session.CloseAsync().Wait();
            }
        }

        public FamilyTreeNode? GetParent(FamilyTreeNode node)
        {
            string query = @"
                MATCH (parent:FamilyTreeNode)-[:PARENT_OF]->(child:FamilyTreeNode {id: $id})
                RETURN parent
                LIMIT 1";
            IAsyncSession session = driver.AsyncSession();
            try
            {
                return Task.Run(async() =>
                {
                    IResultCursor cursor = await session.RunAsync(query, new {id = node.Id.ToString()});
                    IList<IRecord> records = await cursor.ToListAsync();
                    if (records.Count == 0)
                    {
                        return null;
                    }
                    INode parent = records[0]["parent"].As<INode>();
                    return ToFamilyTreeNode(parent);
                }).Result;
            }
            finally
            {
                session.CloseAsync().Wait();
            }
        }

        public void UpdateNode(FamilyTreeNode node)
        {
            string query = @"
            MATCH (n:FamilyTreeNode {id: $id})
            SET n.inheritedFamilyNames = $inheritedFamilyNames,
                n.memberId = $memberId,
                n.inLawId = $inLawId,
                n.dynamicId = $dynamicId;";
            IAsyncSession session = driver.AsyncSession();
            Task.Run(async() =>
            {
                await session.RunAsync(query, new
                {
                    id = node.Id.ToString(),
                    inheritedFamilyNames = node.InheritedFamilyNames,
                    memberId = node.MemberId.ToString(),
                    inLawId = node.InLawId?.ToString() ?? null,
                    dynamicId = node.DynamicId?.ToString() ?? null
                });
            }).Wait();
            session.CloseAsync().Wait();
        }

        private static FamilyTreeNode ToFamilyTreeNode(INode node)
        {
            IReadOnlySet<string> schema = new HashSet<string>()
            {
                "id","inheritedFamilyNames","memberId","inLawId","dynamicId"
            };
            IReadOnlySet<string> currentSchema = node.Properties.Keys.ToHashSet();
            if (!currentSchema.Intersect(schema).ToHashSet().SetEquals(schema))
            {
                throw new InvalidDataException("This schema is invalid.");
            }
            IDictionary<string,BridgeInstance> obj = new Dictionary<string,BridgeInstance>()
            {
                ["id"] = new(node.Get<string>("id")),
                ["inheritedFamilyNames"] = new(node.Get<IEnumerable<string>>("inheritedFamilyNames").Select(f => new BridgeInstance(f))),
                ["memberId"] = new(node.Get<string>("memberId")),
                ["inLawId"] = node.TryGet("inLawId", out string inLawId) ? new(inLawId) : new(),
                ["dynamicId"] = node.TryGet("dynamicId", out string dynamicId) ? new(dynamicId) : new()
            };
            return new(obj);
        }
    }
}