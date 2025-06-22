using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.Data.Comparers
{
    public class NameComparer : IComparer<string>, IEqualityComparer<string>
    {
        public int Compare(string nameA, string nameB)
        {
            bool thisNameIsNull = nameA is null;
            bool otherNameIsNull = nameB is null;
            if (thisNameIsNull && !otherNameIsNull)
            {
                return -1;
            }
            else if (thisNameIsNull && otherNameIsNull)
            {
                return 0;
            }
            else if (!thisNameIsNull && otherNameIsNull)
            {
                return 1;
            }
            string[] nameParts = nameA.Split(' ');
            string[] otherNameParts = nameB.Split(' ');
            return nameParts.Intersect(otherNameParts).Count() >= 2 ? 0 : nameA.CompareTo(nameB);
        }

        public bool Equals(string nameA, string nameB)
        {
            return Compare(nameA, nameB) == 0;
        }

        public int GetHashCode(string name)
        {
            return name.GetHashCode();
        }
    }
}