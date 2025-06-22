using FamilyTreeLibrary.Serialization;

namespace FamilyTreeLibrary.Data.Models
{
    public readonly struct InheritedFamilyName : IBridge, IEquatable<InheritedFamilyName>
    {
        public InheritedFamilyName(BridgeInstance instance)
        {
            string text = instance.AsString;
            string[] segments = text.Split('-');
            Name = segments[0];
            Id = Convert.ToInt32(segments[1]);
        }

        public InheritedFamilyName(string name)
        {
            Name = name;
            Id = new Random().Next();
        }

        public InheritedFamilyName(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public readonly string Name
        {
            get;
        }

        public readonly int Id
        {
            get;
        }

        public readonly BridgeInstance Instance
        {
            get
            {
                return new(Text);
            }
        }

        private readonly string Text
        {
            get
            {
                return $"{Name}-{Id}";
            }
        }

        public readonly bool Equals(InheritedFamilyName other)
        {
            return Text == other.Text;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is InheritedFamilyName other && Equals(other);
        }

        public readonly override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        public readonly override string ToString()
        {
            return Text;
        }

        public static bool operator==(InheritedFamilyName a, InheritedFamilyName b)
        {
            return a.Equals(b);
        }

        public static bool operator!=(InheritedFamilyName a, InheritedFamilyName b)
        {
            return !a.Equals(b);
        }
    }
}