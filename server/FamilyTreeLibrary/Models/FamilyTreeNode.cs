using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Data.Models;
using FamilyTreeLibrary.Serialization;

namespace FamilyTreeLibrary.Models
{
    public class FamilyTreeNode : ICopyable<FamilyTreeNode>, IEquatable<FamilyTreeNode>
    {
        private readonly IDictionary<string, object> vertex;

        public FamilyTreeNode(IDictionary<string, object> nodeObj)
        {
            IEnumerable<string> existingAttributes = new HashSet<string>(){"id", "inheritedFamilyNames", "memberId", "dynamicId"};
            DataUtils.ValidateRequiredAttributes(existingAttributes, nodeObj.Keys);
            IEnumerable<string> schema = existingAttributes.Union(new HashSet<string>{"inLawId"});
            vertex = new Dictionary<string,object>(nodeObj.Where((pair) => schema.Contains(pair.Key)));
        }

        public FamilyTreeNode(ISet<InheritedFamilyName> inheritedFamilyNames, Guid memberId, Guid dynamicId, Guid? inLawId = null)
        {
            vertex = new Dictionary<string,object>()
            {
                ["id"] = Guid.NewGuid().ToString(),
                ["inheritedFamilyNames"] = ListInheritedFamilyNames(inheritedFamilyNames),
                ["memberId"] = memberId.ToString(),
                ["dynamicId"] = dynamicId.ToString()
            };
            if (inLawId.HasValue)
            {
                vertex["inLawId"] = inLawId.Value.ToString();
            }
        }

        public FamilyTreeNode(Guid id, ISet<InheritedFamilyName> inheritedFamilyNames, Guid memberId, Guid dynamicId, Guid? inLawId = null)
        {
            vertex = new Dictionary<string,object>()
            {
                ["id"] = id.ToString(),
                ["inheritedFamilyNames"] = ListInheritedFamilyNames(inheritedFamilyNames),
                ["memberId"] = memberId.ToString(),
                ["dynamicId"] = dynamicId.ToString()
            };
            if (inLawId.HasValue)
            {
                vertex["inLawId"] = inLawId.Value.ToString();
            }
        }

        public Guid Id
        {
            get
            {
                string idValue = vertex["id"].ToString() ?? throw new MissingRequiredAttributeException("id");
                return Guid.Parse(idValue);
            }
        }

        public ISet<InheritedFamilyName> InheritedFamilyNames
        {
            get
            {
                IEnumerable<string> inheritedFamilyNamesValue = (IEnumerable<string>)vertex["inheritedFamilyNames"];
                return inheritedFamilyNamesValue.Select((inheritedFamilyNameValue) =>
                {
                    BridgeInstance instance = new(inheritedFamilyNameValue);
                    return new InheritedFamilyName(instance);
                }).ToHashSet();
            }
            set
            {
                vertex["inheritedFamilyNames"] = ListInheritedFamilyNames(value);
            }
        }

        public Guid MemberId
        {
            get
            {
                string memberIdValue = vertex["memberId"].ToString() ?? throw new MissingRequiredAttributeException("memberId");
                return Guid.Parse(memberIdValue);
            }
        }

        public Guid? InLawId
        {
            get
            {
                if (!vertex.TryGetValue("inLawId", out object? inLawIdValue))
                {
                    return null;
                }
                string inLawId = inLawIdValue.ToString() ?? throw new ArgumentNullException(nameof(inLawId), "InLawId must be specified, since it exists.");
                return Guid.Parse(inLawId);
            }
            set
            {
                if (value.HasValue)
                {
                    vertex["inLawId"] = value.Value.ToString();
                }
                else
                {
                    vertex.Remove("inLawId");
                }
            }
        }

        public Guid DynamicId
        {
            get
            {
                string dynamicIdValue = vertex["dynamicId"].ToString() ?? throw new MissingRequiredAttributeException("dynamicId");
                return Guid.Parse(dynamicIdValue);
            }
        }

        public IDictionary<string,object> Vertex
        {
            get
            {
                return vertex;
            }
        }

        public FamilyTreeNode Copy()
        {
            return new(vertex);
        }
        public bool Equals(FamilyTreeNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return MemberId == other.MemberId && InLawId == other.InLawId && DynamicId == other.DynamicId;
        }

        public override bool Equals(object? obj)
        {
            return obj is FamilyTreeNode other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MemberId, InLawId, DynamicId);
        }

        public override string ToString()
        {
            string representation = $"{Id} -> Inherited Family Names: " + "{" + string.Join(',', ListInheritedFamilyNames(InheritedFamilyNames)) + "}; " + $"Member Id: {MemberId};";
            if (InLawId.HasValue)
            {
                representation += $" In-Law Id: {InLawId.Value};";
            }
            representation += $" Dynamic Id: {DynamicId}";
            return representation.TrimEnd(';');
        }

        private static IEnumerable<string> ListInheritedFamilyNames(IEnumerable<InheritedFamilyName> inheritedFamilyNames)
        {
            return inheritedFamilyNames.Select((inheritedFamilyName) => inheritedFamilyName.Instance.AsString);
        }
    }
}