using FamilyTreeLibrary.Data.Comparers;
using FamilyTreeLibrary.Exceptions;
using MongoDB.Bson;

namespace FamilyTreeLibrary.Models
{
    public class Person : ICloneable, IComparable<Person>, IEquatable<Person>
    {
        private FamilyTreeDate deceasedDate;

        public Person(string name, FamilyTreeDate birthDate, FamilyTreeDate deceasedDate)
        {
            Name = name;
            BirthDate = birthDate;
            DeceasedDate = deceasedDate;
        }

        public Person(BsonDocument document)
        {
            Name = document[nameof(Name)].IsBsonNull ? null : document[nameof(Name)].AsString;
            BirthDate = document[nameof(BirthDate)].IsBsonNull ? FamilyTreeDate.DefaultDate : new(document[nameof(BirthDate)].AsString);
            DeceasedDate = document[nameof(DeceasedDate)].IsBsonNull ? FamilyTreeDate.DefaultDate : new(document[nameof(DeceasedDate)].AsString);
        }

        public Person(string representation)
        {
            if (representation is null || representation == "")
            {
                Name = null;
                BirthDate = FamilyTreeDate.DefaultDate;
                DeceasedDate = FamilyTreeDate.DefaultDate;
            }
            else
            {
                string[] delimiters = {" (", " - ", ")"};
                string[] parts = representation.Split(delimiters, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                Name = parts[0];
                BirthDate = parts.Length == 3 ? new FamilyTreeDate(parts[1]) : FamilyTreeDate.DefaultDate;
                DeceasedDate = parts.Length == 3 && parts[2] != "Present" ? new FamilyTreeDate(parts[2]) : FamilyTreeDate.DefaultDate;
            }
        }
        public string Name
        {
            get;
        }
        public FamilyTreeDate BirthDate
        {
            get;
            set;
        }
        public FamilyTreeDate DeceasedDate
        {
            get
            {
                return deceasedDate;
            }
            set
            {
                if (value.CompareTo(FamilyTreeDate.DefaultDate) != 0 && value.CompareTo(BirthDate) < 0)
                {
                    throw new DeceasedDateException(this, value);
                }
                deceasedDate = value;
            }
        }

        public BsonDocument Document
        {
            get
            {
                Dictionary<string,object> doc = new()
                {
                    {nameof(Name), Name is null ? BsonNull.Value : Name},
                    {nameof(BirthDate), BirthDate == default || BirthDate == FamilyTreeDate.DefaultDate ? 
                        BsonNull.Value : BirthDate.ToString()},
                    {nameof(DeceasedDate), DeceasedDate == default || DeceasedDate == FamilyTreeDate.DefaultDate ? 
                        BsonNull.Value : DeceasedDate.ToString()}
                };
                return new(doc);
            }
        }

        public static Person EmptyPerson
        {
            get
            {
                return new(null, FamilyTreeDate.DefaultDate, FamilyTreeDate.DefaultDate);
            }
        }

        public object Clone()
        {
            return new Person(Document);
        }

        public int CompareTo(Person other)
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
            else if (DeceasedDate < other.DeceasedDate)
            {
                return -1;
            }
            else if (DeceasedDate > other.DeceasedDate)
            {
                return 1;
            }
            IComparer<string> nameCompare = new NameComparer();
            return nameCompare.Compare(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            return obj is Person other && Equals(other);
        }

        public bool Equals(Person other)
        {
            return CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BirthDate, DeceasedDate, Name);
        }

        public override string ToString()
        {
            if (Name is not null && BirthDate > FamilyTreeDate.DefaultDate && DeceasedDate > BirthDate)
            {
                return $"{Name} ({BirthDate} - {DeceasedDate})";
            }
            else if (Name is not null && BirthDate > FamilyTreeDate.DefaultDate)
            {
                return $"{Name} ({BirthDate} - Present)";
            }
            else if (Name is not null)
            {
                return Name;
            }
            return null;
        }

        public static bool operator== (Person a, Person b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (aIsNull && bIsNull) || (!aIsNull && a.Equals(b)); 
        }

        public static bool operator!= (Person a, Person b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (!aIsNull || !bIsNull) && (aIsNull || !a.Equals(b));
        }

        public static bool operator< (Person a, Person b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (aIsNull && !bIsNull) || (!aIsNull && a.CompareTo(b) < 0);
        }

        public static bool operator> (Person a, Person b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (!aIsNull && bIsNull) || (!aIsNull && a.CompareTo(b) > 0);
        }
    }
}