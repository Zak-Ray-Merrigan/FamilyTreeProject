using FamilyTreeLibrary.Data.Models;
using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Models;
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

        public void Connect(FamilyTreeNode parent, FamilyTreeNode child)
        {
            string query = @"
                MATCH (parent:FamilyTreeNode {id: $parentId})
                MATCH (child:FamilyTreeNode {id: $childId})
                MERGE (parent)-[:PARENT_CHILD]->(child);";
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
                    memberId: $memberId";
            if (node.InLawId.HasValue)
            {
                query += @",
                    inLawId: $inLawId";
            }
            if (node.DynamicId.HasValue)
            {
                query += @",
                    dynamicId: $dynamicId";
            }
            query += @"
                });";
            IAsyncSession session = driver.AsyncSession();
            Task.Run(async () =>
            {
                await session.RunAsync(query, node.Vertex);
            }).Wait();
            session.CloseAsync().Wait();
        }

        public IEnumerable<FamilyTreeNode> GetChildren(FamilyTreeNode node)
        {
            string query = @"
                MATCH (parent:FamilyTreeNode {id: $id})-[:PARENT_CHILD]->(child:FamilyTreeNode)
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

        public IEnumerable<FamilyTreeNode> GetParents(FamilyTreeNode node)
        {
            string query = @"
                MATCH (parent:FamilyTreeNode)-[:PARENT_CHILD]->(child:FamilyTreeNode {id: $id})
                RETURN parent;";
            IAsyncSession session = driver.AsyncSession();
            try
            {
                return Task.Run(async() =>
                {
                    IResultCursor cursor = await session.RunAsync(query, new {id = node.Id.ToString()});
                    IReadOnlyList<IRecord> records = await cursor.ToListAsync();
                    if (records.Count < 1)
                    {
                        return [];
                    }
                    else if (records.Count > 2)
                    {
                        throw new InvalidOperationException("This node must have at most 2 parents.");
                    }
                    return records.Select((record) =>
                    {
                        INode parent = record["parent"].As<INode>();
                        return ToFamilyTreeNode(parent);
                    });
                }).Result;
            }
            finally
            {
                session.CloseAsync().Wait();
            }
        }

        public IEnumerable<FamilyTreeNode> GetRootNodesByFamilyName(InheritedFamilyName familyName)
        {
            string query = @"
                MATCH (n:FamilyTreeNode)
                WHERE NOT (():FamilyTreeNode)-[:PARENT_CHILD]->(n)
                    AND $inheritedFamilyName IN n.inheritedFamilyNames
                RETURN n;";
            IDictionary<string,object> parameters = new Dictionary<string, object>
            {
                { "inheritedFamilyName", familyName.ToString() }
            };

            IAsyncSession session = driver.AsyncSession();
            IEnumerable<FamilyTreeNode> result = Task.Run(async () =>
            {
                IResultCursor cursor = await session.RunAsync(query, parameters);
                IList<IRecord> records = await cursor.ToListAsync();
                return records.Select(record => ToFamilyTreeNode(record.As<INode>()));
            }).Result;

            session.CloseAsync().Wait();
            return result;
        }

        public void RemoveNode(FamilyTreeNode node)
        {
            string query = @"MATCH (node:FamilyTreeNode) {id: $id}
                DETACH DELETE node;";
            IAsyncSession session = driver.AsyncSession();
            Task.Run(async() =>
            {
                await session.RunAsync(query, node.Id.ToString());
            });
        }

        public void UpdateNode(FamilyTreeNode node)
        {
            string query = @"
            MATCH (n:FamilyTreeNode {id: $id})
            SET n.inheritedFamilyNames = $inheritedFamilyNames,
                n.memberId = $memberId";
            if (node.InLawId.HasValue)
            {
                query += @",
                    n.inLawId = $inLawId";
            }
            if (node.DynamicId.HasValue)
            {
                query += @",
                    n.dynamicId = $dynamicId";
            }
            query += ";";
            IAsyncSession session = driver.AsyncSession();
            Task.Run(async() =>
            {
                await session.RunAsync(query, node.Vertex);
            }).Wait();
            session.CloseAsync().Wait();
        }

        private static FamilyTreeNode ToFamilyTreeNode(INode node)
        {
            return new FamilyTreeNode(new Dictionary<string,object>(node.Properties));
        }
    }
}