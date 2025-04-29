using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Data.Models
{
    public class Template
    {
        public required IReadOnlyCollection<Line> Family
        {
            get;
            set;
        }

        public required InheritedFamilyName FamilyName
        {
            get;
            set;
        }

        public override string ToString()
        {
            return $"Inherited Family Name: {FamilyName}\n{string.Join('\n', Family.Select(f => f.ToString()))}";
        }
    }
}