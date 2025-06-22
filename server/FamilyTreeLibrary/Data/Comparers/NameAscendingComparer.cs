using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Data.Comparers
{
    public class NameAscendingComparer : IComparer<Family>
    {
        public int Compare(Family a, Family b)
        {
            IComparer<string> memberNameCompare = new NameComparer();
            int memberNameCompareResult = memberNameCompare.Compare(a.Member.Name, b.Member.Name);
            if (memberNameCompareResult != 0)
            {
                return memberNameCompareResult;
            }
            bool familyAHasInlaw = a.InLaw is not null;
            bool familyBHasInlaw = b.InLaw is not null;
            if (!familyAHasInlaw && !familyBHasInlaw)
            {
                return 0;
            }
            else if (!familyAHasInlaw && familyBHasInlaw)
            {
                return -1;
            }
            else if (familyAHasInlaw && !familyBHasInlaw)
            {
                return 1;
            }
            return a.InLaw.Name.CompareTo(b.InLaw.Name);
        }
    }
}