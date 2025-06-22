using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Data.PDF.Models
{
    public class Line : ICloneable
    {
        public Line(string name = null, Queue<FamilyTreeDate> dates = default)
        {
            Name = name;
            Dates = dates == default ? new() : dates;
        }
        public string Name
        {
            get;
            set;
        }

        public Queue<FamilyTreeDate> Dates
        {
            get;
        }

        public object Clone()
        {
            return new Line(Name,Dates);
        }

        public override bool Equals(object obj)
        {
            return obj != null && ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Dates);
        }

        public override string ToString()
        {
            string result = Name ?? "";
            foreach (FamilyTreeDate d in Dates)
            {
                result += $" {d.ToString() ?? ""}";
            }
            return result;
        }
    }
}