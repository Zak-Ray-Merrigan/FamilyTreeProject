namespace FamilyTreeLibrary.Data.PDF.OrderingType.Comparers
{
    public class OrderingTypeComparer : IComparer<AbstractOrderingType[]>, IEqualityComparer<AbstractOrderingType[]>
    {
        public int Compare(AbstractOrderingType[] a, AbstractOrderingType[] b)
        {
            for (int i = 0; i < Math.Min(a.Length, b.Length); i++)
            {
                if (a[i].CompareTo(b[i]) < 0)
                {
                    return -1;
                }
                else if (a[i].CompareTo(b[i]) > 0)
                {
                    return 1;
                }
            }
            return a.Length - b.Length;
        }

        public bool Equals(AbstractOrderingType[] a, AbstractOrderingType[] b)
        {
            return Compare(a,b) == 0;
        }

        public int GetHashCode(AbstractOrderingType[] orderingType)
        {
            return HashCode.Combine(orderingType);
        }
    }
}