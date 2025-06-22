namespace FamilyTreeLibrary.Serialization
{
    public abstract class AbstractBridge : IBridge, IEquatable<AbstractBridge>
    {
        public abstract BridgeInstance Instance{ get; }

        public abstract bool Equals(AbstractBridge? other);

        public override bool Equals(object? obj)
        {
            return obj is AbstractBridge bridge && Equals(bridge);
        }

        public override int GetHashCode()
        {
            return Instance.GetHashCode();
        }

        public override string ToString()
        {
            return Instance.ToString();
        }

        public static bool operator ==(AbstractBridge? left, AbstractBridge? right)
        {
            return left is null ? right is null : left.Equals(right);
        }

        public static bool operator !=(AbstractBridge? left, AbstractBridge? right)
        {
            return left is null ? right is not null : !left.Equals(right);
        }
    }
}