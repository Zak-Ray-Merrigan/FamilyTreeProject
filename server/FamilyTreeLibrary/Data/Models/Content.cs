namespace FamilyTreeLibrary.Data.Models
{
    public readonly struct Content(Line header, string subContent) : ICopyable<Content>, IEquatable<Content>
    {
        public Line Header
        {
            get => header;
        }

        public string SubContent
        {
            get => subContent;
        }

        public Content Copy()
        {
            return new(Header, SubContent);
        }

        public bool Equals(Content other)
        {
            return Header == other.Header && SubContent == other.SubContent;
        }

        public override bool Equals(object? obj)
        {
            return obj is Content other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Header, SubContent);
        }

        public override string ToString()
        {
            return $"Header: {Header}\nSubContent:\n{SubContent}";
        }

        public static bool operator==(Content a, Content b)
        {
            return a.Equals(b);
        }

        public static bool operator!=(Content a, Content b)
        {
            return !a.Equals(b);
        }
    }
}