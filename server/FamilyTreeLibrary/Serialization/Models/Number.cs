namespace FamilyTreeLibrary.Serialization.Models
{
    public readonly struct Number(double value = 0)
    {
        private readonly double value = value;

        public double AsDouble
        {
            get
            {
                return value;
            }
        }

        public long AsLong
        {
            get
            {
                if (Math.Ceiling(value) != Math.Floor(value))
                {
                    throw new InvalidCastException($"{value} isn't an integer.");
                }
                return (long)value;
            }
        }

        public int AsInt
        {
            get
            {
                long l = AsLong;
                if (l < int.MinValue || l > int.MaxValue)
                {
                    throw new InvalidCastException($"{value} isn't within the range of {int.MinValue} and {int.MaxValue}");
                }
                return (int)l;
            }
        }

        public bool IsLong
        {
            get
            {
                return Math.Floor(value) == Math.Ceiling(value);
            }
        }

        public bool IsInt
        {
            get
            {
                return Math.Floor(value) == Math.Ceiling(value) && value >= int.MinValue && value <= int.MaxValue;
            }
        }

        public bool TryGetLong(out long l)
        {
            bool result = IsLong;
            l = result ? AsLong : 0;
            return result;
        }

        public bool TryGetInt(out int i)
        {
            bool result = IsInt;
            i = result ? AsInt : 0;
            return result;
        }
    }
}