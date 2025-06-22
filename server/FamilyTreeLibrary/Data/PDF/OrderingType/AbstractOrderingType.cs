namespace FamilyTreeLibrary.Data.PDF.OrderingType
{
    public abstract class AbstractOrderingType : IComparable<AbstractOrderingType>
    {
        protected AbstractOrderingType(int key, int generation, int maxKey)
        {
            Generation = generation;
            MaxKey = maxKey;
            ConversionPair = new(key, FindValue(key));
        }

        protected AbstractOrderingType(string value, int generation, int maxKey)
        {
            Generation = generation;
            MaxKey = maxKey;
            ConversionPair = new(FindKey(value), value);
        }

        public KeyValuePair<int,string> ConversionPair
        {
            get;
        }

        protected int MaxKey
        {
            get;
        }

        protected OrderingTypeTypes Type
        {
            get
            {
                return Generation switch
                {
                    1 => OrderingTypeTypes.RomanNumeralUpper,
                    2 => OrderingTypeTypes.CapitalLetter,
                    3 => OrderingTypeTypes.Numbering,
                    4 => OrderingTypeTypes.LowerCaseLetter,
                    5 => OrderingTypeTypes.ParenthesizedNumbering,
                    6 => OrderingTypeTypes.RomanNumeralLower,
                    _ => throw new NotSupportedException($"{Generation} isn't supported.")
                };
            }
        }

        private int Generation
        {
            get;
        }

        public int CompareTo(AbstractOrderingType other)
        {
            int temp = Type.CompareTo(other.Type);
            return temp == 0 ? ConversionPair.Key - other.ConversionPair.Key : temp;
        }

        public override bool Equals(object obj)
        {
            return obj != null && ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ConversionPair);
        }

        public static AbstractOrderingType GetOrderingType(int key, int generation, int maxKey = int.MaxValue)
        {
            return generation switch
            {
                1 => new RomanNumeralOrderingType(key, true, maxKey),
                2 => new LetterOrderingType(key, true, maxKey),
                3 => new DigitOrderingType(key, false, maxKey),
                4 => new LetterOrderingType(key, false, maxKey),
                5 => new ParenthesizedDigitOrderingType(key, maxKey),
                6 => new RomanNumeralOrderingType(key, false, maxKey),
                _ => null
            };
        }

        public static Queue<AbstractOrderingType> GetOrderingTypeByLine(string line, int maxKey = int.MaxValue)
        {
            Queue<AbstractOrderingType> result = new();
            string token = line.Split(' ')[0];
            for (int generation = 1; generation <= 6; generation++)
            {
                if (TryGetOrderingType(out AbstractOrderingType orderingType, token, generation, maxKey))
                {
                    result.Enqueue(orderingType);
                }
            }
            return result;
        }

        public static AbstractOrderingType[] NextOrderingType(AbstractOrderingType[] temp, AbstractOrderingType orderingType)
        {
            if (temp.Length == 0)
            {
                return IncrementGeneration(Array.Empty<AbstractOrderingType>());
            }
            ICollection<AbstractOrderingType[]> possibleNexts = new List<AbstractOrderingType[]>();
            if (temp.Length < 6)
            {
                possibleNexts.Add(IncrementGeneration(temp));
            }
            possibleNexts.Add(ReplaceWithIncrementByKey(temp));
            AbstractOrderingType[] previous = temp;
            while (true)
            {
                previous = PreviousOrderingType(previous);
                if (previous.Length == 0)
                {
                    break;
                }
                possibleNexts.Add(ReplaceWithIncrementByKey(previous));
            }
            foreach (AbstractOrderingType[] value in possibleNexts)
            {
                if (value[^1].Equals(orderingType))
                {
                    return value;
                }
            }
            return null;
        }

        public static AbstractOrderingType[] PreviousOrderingType(AbstractOrderingType[] current)
        {
            AbstractOrderingType[] previous = new AbstractOrderingType[current.Length - 1];
            Array.Copy(current, previous, current.Length - 1);
            return previous;
        }

        public override string ToString()
        {
            return ConversionPair.ToString();
        }

        public static bool TryGetOrderingType(out AbstractOrderingType orderingType, string value, int generation, int maxKey = int.MaxValue)
        {
            switch (generation)
            {
                case 1: orderingType = new RomanNumeralOrderingType(value, true, maxKey); break;
                case 2: orderingType = new LetterOrderingType(value, true, maxKey); break;
                case 3: orderingType = new DigitOrderingType(value, false, maxKey); break;
                case 4: orderingType = new LetterOrderingType(value, false, maxKey); break;
                case 5: orderingType = new ParenthesizedDigitOrderingType(value, maxKey); break;
                case 6: orderingType = new RomanNumeralOrderingType(value, false, maxKey); break;
                default: orderingType = null; return false; 
            }
            return orderingType.ConversionPair.Key > 0 && orderingType.ConversionPair.Value != ""; 
        }

        protected abstract int FindKey(string value);

        protected abstract string FindValue(int key);


        private static AbstractOrderingType[] IncrementGeneration(AbstractOrderingType[] temp)
        {
            IList<AbstractOrderingType> collection = temp.ToList();
            collection.Add(GetOrderingType(1, temp.Length + 1));
            return collection.ToArray();
        }

        private static AbstractOrderingType[] ReplaceWithIncrementByKey(AbstractOrderingType[] temp)
        {
            AbstractOrderingType[] collection = new AbstractOrderingType[temp.Length];
            if (temp.Length == 1)
            {
                collection[0] = GetOrderingType(temp[0].ConversionPair.Key + 1, temp.Length);
            }
            else
            {
                Array.Copy(temp, collection, temp.Length - 1);
                collection[^1] = GetOrderingType(temp[^1].ConversionPair.Key + 1, temp.Length);
            }
            return collection;
        }
    }
}