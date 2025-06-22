namespace FamilyTreeLibrary.Serialization
{
    public abstract class AbstractComparableBridge : AbstractBridge, IComparable<AbstractComparableBridge>, IEquatable<AbstractComparableBridge>
    {
        public abstract override BridgeInstance Instance { get;}

        public abstract int CompareTo(AbstractComparableBridge? other);

        public override bool Equals(AbstractBridge? other)
        {
            return other is AbstractComparableBridge obj && Equals(obj);
        }
        public bool Equals(AbstractComparableBridge? other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object? obj)
        {
            return obj is AbstractComparableBridge other && Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public static bool operator<(AbstractComparableBridge? a, AbstractComparableBridge? b)
        {
            return a is null ? b is not null : a.CompareTo(b) < 0;
        }

        public static bool operator>(AbstractComparableBridge? a, AbstractComparableBridge? b)
        {
            return a is not null && a.CompareTo(b) > 0;
        }

        public static bool operator<=(AbstractComparableBridge? a, AbstractComparableBridge? b)
        {
            return a is null ? b is null : a.CompareTo(b) <= 0;
        }

        public static bool operator>=(AbstractComparableBridge? a, AbstractComparableBridge? b)
        {
            return a is not null && a.CompareTo(b) >= 0;
        }
    }
}