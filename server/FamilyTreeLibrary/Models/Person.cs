using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Serialization;
using System.Text.Json;

namespace FamilyTreeLibrary.Models
{
    public class Person : AbstractComparableBridge, IComparable<Person>, ICopyable<Person>, IEquatable<Person>
    {
        private readonly IDictionary<string, BridgeInstance> document;
        private static readonly IEnumerable<string> requiredAttributes = ["id", "birthName", "birthDate", "deceasedDate"];

        public Person(IDictionary<string, BridgeInstance> instance, bool needToGenerateId = false)
        {
            if (!instance.ContainsKey("id") && !needToGenerateId)
            {
                throw new UniqueIdentifierNotExistsException("An id must be present to uniquely identify a person document.");
            }
            document = instance;
            if (!document.ContainsKey("id"))
            {
                document["id"] = new(Guid.NewGuid().ToString());
            }
        }

        public Guid Id
        {
            get
            {
                return Guid.Parse(document["id"].AsString);
            }
        }

        public string BirthName
        {
            get
            {
                return document["birthName"].AsString;
            }
        }

        public FamilyTreeDate? BirthDate
        {
            get
            {
                return document["birthDate"].TryGetString(out string birthDate) ? new(birthDate) : null;
            }
            set
            {
                document["birthDate"] = value is null ? new() : value.Instance;
            }
        }

        public FamilyTreeDate? DeceasedDate
        {
            get
            {
                return document["deceasedDate"].TryGetString(out string deceasedDate) ? new(deceasedDate) : null;
            }
            set
            {
                document["deceasedDate"] = value is null ? new() : value.Instance;
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
            return CompareTo(other as Person);
        }

        public int CompareTo(Person? p)
        {
            if (p is null)
            {
                return 1;
            }
            else if (BirthDate < p.BirthDate)
            {
                return -1;
            }
            else if (BirthDate > p.BirthDate)
            {
                return 1;
            }
            else if (DeceasedDate < p.DeceasedDate)
            {
                return -1;
            }
            else if (DeceasedDate > p.DeceasedDate)
            {
                return 1;
            }
            return BirthName.CompareTo(p.BirthName);
        }

        public Person Copy()
        {
            return new(document);
        }

        public bool Equals(Person? other)
        {
            return base.Equals(other);
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            bool birthDateDefined = BirthDate is not null;
            bool deceasedDateDefined = DeceasedDate is not null;
            if (!(birthDateDefined || deceasedDateDefined))
            {
                return BirthName.GetHashCode();
            }
            else if (birthDateDefined && !deceasedDateDefined)
            {
                return HashCode.Combine(BirthName, BirthDate);
            }
            return HashCode.Combine(BirthName, BirthDate, DeceasedDate);
        }

        public override string ToString()
        {
            string value = BirthName + " ";
            const char EN_DASH = '\u2013';
            if (BirthDate is null && DeceasedDate is null)
            {
                value += $"({EN_DASH})";
            }
            else if (DeceasedDate is null)
            {
                value += $"({BirthDate} {EN_DASH} Present)";
            }
            else
            {
                value += $"({BirthDate} {EN_DASH} {DeceasedDate})";
            }
            return value;
        }
    }
}