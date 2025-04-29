using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Serialization;

namespace FamilyTreeLibrary.Models
{
    public class FamilyDynamic : AbstractComparableBridge, IComparable<FamilyDynamic>, ICopyable<FamilyDynamic>, IEquatable<FamilyDynamic>
    {
        private readonly IDictionary<string, BridgeInstance> document;
        private static readonly IEnumerable<string> requiredAttributes = new HashSet<string>(){"id", "familyDynamicStartDate", "pageTitle"};

        public FamilyDynamic(IDictionary<string,BridgeInstance> obj, bool needToGenerateId = false)
        {
            if (!obj.ContainsKey("id") && !needToGenerateId)
            {
                throw new UniqueIdentifierNotExistsException("An id must be present to uniquely identify a family dynamic document.");
            }
            document = obj;
            if (!document.ContainsKey("id"))
            {
                obj["id"] = new(Guid.NewGuid().ToString());
            }
        }

        public Guid Id
        {
            get
            {
                return Guid.Parse(document["id"].AsString);
            }
        }

        public FamilyTreeDate FamilyDynamicStartDate
        {
            get
            {
                return new(document["familyDynamicStartDate"].AsString);
            }
        }

        public string PageTitle
        {
            get
            {
                return document["pageTitle"].AsString;
            }
        }

        public BridgeInstance this[string attribute]
        {
            get
            {
                return document[attribute];
            }
            set
            {
                FamilyTreeUtils.ValidateExtendedAttributeAccessibility(requiredAttributes, attribute);
                document[attribute] = value;
            }
        }

        public override BridgeInstance Instance => new(document);

        public override int CompareTo(AbstractComparableBridge? other)
        {
            return CompareTo(other as FamilyDynamic);
        }

        public int CompareTo(FamilyDynamic? p)
        {
            if (p is null)
            {
                return 1;
            }
            return FamilyDynamicStartDate.CompareTo(p.FamilyDynamicStartDate);
        }

        public FamilyDynamic Copy()
        {
            return new(document);
        }

        public bool Equals(FamilyDynamic? other)
        {
            return base.Equals(other);
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FamilyDynamicStartDate, PageTitle);
        }

        public override string ToString()
        {
            return FamilyDynamicStartDate.ToString();
        }
    }
}