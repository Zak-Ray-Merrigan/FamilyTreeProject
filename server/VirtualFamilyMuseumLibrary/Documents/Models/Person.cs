using System.Text;
using VirtualFamilyMuseumLibrary.Models;
using VirtualFamilyMuseumLibrary.Serialization;
using VirtualFamilyMuseumLibrary.Serialization.Models;

namespace VirtualFamilyMuseumLibrary.Documents.Models
{
    public class Person : IBridge, IComparable<Person>, IEquatable<Person>
    {
        private readonly IDictionary<string,BridgeValue> instance;

        public Person(IDictionary<string,BridgeValue> obj)
        {
            instance = new Dictionary<string,BridgeValue>(obj);
        }

        public Person(Person p)
        {
            instance = new Dictionary<string,BridgeValue>(p.instance);
        }

        public Guid Id
        {
            get
            {
                string id = (string)instance[DocumentExtensions.PersonRequiredAttributeNames[PersonRequiredAttributes.ID]];
                return Guid.Parse(id);
            }
            set
            {
                string id = value.ToString();
                instance[DocumentExtensions.PersonRequiredAttributeNames[PersonRequiredAttributes.ID]] = id;
            }
        }

        public string BirthFamilyName
        {
            get
            {
                return (string)instance[DocumentExtensions.PersonRequiredAttributeNames[PersonRequiredAttributes.BIRTH_FAMILY_NAME]];
            }
            set
            {
                instance[DocumentExtensions.PersonRequiredAttributeNames[PersonRequiredAttributes.BIRTH_FAMILY_NAME]] = value;
            }
        }

        public string BirthName
        {
            get
            {
                return (string)instance[DocumentExtensions.PersonRequiredAttributeNames[PersonRequiredAttributes.BIRTH_NAME]];
            }
            set
            {
                instance[DocumentExtensions.PersonRequiredAttributeNames[PersonRequiredAttributes.BIRTH_NAME]] = value;
            }
        }

        public FamilyDate? BirthDate
        {
            get
            {
                BridgeValue birthDate = instance[DocumentExtensions.PersonRequiredAttributeNames[PersonRequiredAttributes.BIRTH_DATE]];
                return birthDate.TryAsString(out string? text) && text is not null ? FamilyDate.GetDate(text) : null;
            }
            set
            {
                instance[DocumentExtensions.PersonRequiredAttributeNames[PersonRequiredAttributes.BIRTH_DATE]] = value.HasValue ? value.Value.Value : new();
            }
        }

        public FamilyDate? DeceasedDate
        {
            get
            {
                BridgeValue deceasedDate = instance[DocumentExtensions.PersonRequiredAttributeNames[PersonRequiredAttributes.DECEASED_DATE]];
                return deceasedDate.TryAsString(out string? text) && text is not null ? FamilyDate.GetDate(text) : null;
            }
            set
            {
                instance[DocumentExtensions.PersonRequiredAttributeNames[PersonRequiredAttributes.DECEASED_DATE]] = value.HasValue ? value.Value.Value : new();
            }
        }

        public BridgeValue this[string attribute]
        {
            get
            {
                return instance[attribute];
            }
            set
            {
                instance[attribute] = value;
            }
        }

        public BridgeValue Value
        {
            get => new(new Dictionary<string,BridgeValue>(instance));
        }

        public int CompareTo(Person? other)
        {
            if (other is null)
            {
                return 1;
            }
            else if (BirthDate < other.BirthDate)
            {
                return -1;
            }
            else if (BirthDate > other.BirthDate)
            {
                return 1;
            }
            int birthNameCompare = string.CompareOrdinal(BirthName, other.BirthName);
            if (birthNameCompare != 0)
            {
                return birthNameCompare;
            }
            else if (DeceasedDate < other.DeceasedDate)
            {
                return -1;
            }
            else if (DeceasedDate > other.DeceasedDate)
            {
                return 1;
            }
            return 0;
        }

        public bool Equals(Person? other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object? obj)
        {
            return obj is Person other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BirthName, BirthDate, DeceasedDate);
        }

        public override string ToString()
        {
            StringBuilder result = new($"{BirthName} ");
            if (!BirthDate.HasValue)
            {
                result.Append($"({Extensions.EN_DASH})");
            }
            else if (!DeceasedDate.HasValue)
            {
                result.Append($"({BirthDate.Value} {Extensions.EN_DASH} Present)");
            }
            else
            {
                result.Append($"({BirthDate.Value} {Extensions.EN_DASH} {DeceasedDate.Value})");
            }
            return result.ToString();
        }

        public static bool operator==(Person? a, Person? b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            else if (a is null && b is not null)
            {
                return false;
            }
            return a!.Equals(b);
        }

        public static bool operator!=(Person? a, Person? b)
        {
            if (a is null && b is null)
            {
                return false;
            }
            else if (a is null && b is not null)
            {
                return true;
            }
            return !a!.Equals(b);
        }

        public static bool operator<(Person? a, Person? b)
        {
            if (a is null && b is null)
            {
                return false;
            }
            else if (a is null && b is not null)
            {
                return true;
            }
            return a!.CompareTo(b) < 0;
        }

        public static bool operator>(Person? a, Person? b)
        {
            if (a is null && b is null)
            {
                return false;
            }
            else if (a is null && b is not null)
            {
                return false;
            }
            return a!.CompareTo(b) > 0;
        }

        public static bool operator<=(Person? a, Person? b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            else if (a is null && b is not null)
            {
                return true;
            }
            return a!.CompareTo(b) <= 0;
        }

        public static bool operator>=(Person? a, Person? b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            else if (a is null && b is not null)
            {
                return false;
            }
            return a!.CompareTo(b) >= 0;
        }
    }
}