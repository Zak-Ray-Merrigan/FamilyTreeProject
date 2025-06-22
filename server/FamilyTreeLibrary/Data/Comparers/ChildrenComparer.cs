using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Data.Comparers
{
    public class ChildrenComparer : IComparer<IEnumerable<Family>>
    {
        public int Compare(IEnumerable<Family> childrenA, IEnumerable<Family> childrenB)
        {
            Family[] collectionA = childrenA.ToArray();
            Family[] collectionB = childrenB.ToArray();

            int length = Math.Min(collectionA.Length, collectionB.Length);
            for (int i = 0; i < length; i++)
            {
                Family a = collectionA[i];
                Family b = collectionB[i];
                if (a < b)
                {
                    return -1;
                }
                else if (a > b)
                {
                    return 1;
                }
            }
            return collectionA.Length - collectionB.Length;
        }
    }
}