using FamilyTreeLibrary.Data.PDF.OrderingType;

namespace FamilyTreeLibrary.Data.PDF.Models
{
    public class Section : ICloneable
    {
        public Section(AbstractOrderingType[] orderingType, FamilyNode node)
        {
            OrderingType = orderingType;
            Node = node;
        }
        public AbstractOrderingType[] OrderingType
        {
            get;
        }

        public FamilyNode Node
        {
            get;
        }

        public object Clone()
        {
            return new Section(OrderingType, Node);
        }

        public override bool Equals(object obj)
        {
            return obj != null && ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OrderingType, Node);
        }

        public override string ToString()
        {
            string orderingTypeOutput = "{" + string.Join<AbstractOrderingType>(',', OrderingType) + "}";
            return $"\nOrderingType: {orderingTypeOutput}\nNode: {Node}";
        }
    }
}