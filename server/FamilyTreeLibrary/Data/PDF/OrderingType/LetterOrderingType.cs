using System.Collections;
using System.Text.RegularExpressions;

namespace FamilyTreeLibrary.Data.PDF.OrderingType
{
    public class LetterOrderingType : AbstractOrderingType
    {
        internal LetterOrderingType(int key, bool isUpperCase, int maxKey = int.MaxValue)
        :base(key, isUpperCase ? 2 : 4, maxKey)
        {
        }

        internal LetterOrderingType(string value, bool isUpperCase, int maxKey = int.MaxValue)
        :base(value, isUpperCase ? 2 : 4, maxKey)
        {
        }

        protected override int FindKey(string value)
        {
            string pattern;
            switch (Type)
            {
                case OrderingTypeTypes.CapitalLetter: pattern = "^[A-Z]+\\.$"; break;
                case OrderingTypeTypes.LowerCaseLetter: pattern = "^[a-z]+\\.$"; break;
                default: return 0;
            }
            if (!Regex.IsMatch(value, pattern))
            {
                return 0;
            }
            int key = 1;
            while (FindValue(key) != value)
            {
                key++;
                if (key > MaxKey)
                {
                    return 0;
                }
            }
            return key;
        }

        protected override string FindValue(int key)
        {
            if (key < 0 || key > MaxKey)
            {
                return "";
            }
            IEnumerator<string> enumerator = new LetterEnumerator();
            for (int k = 1; k < key; k++)
            {
                enumerator.Dispose();
            }
            string value = enumerator.Current;
            switch (Type)
            {
                case OrderingTypeTypes.CapitalLetter: value += "."; break;
                case OrderingTypeTypes.LowerCaseLetter: value = $"{value.ToLower()}."; break;
                default: return "";
            }
            enumerator.Reset();
            return value;
        }

        private class LetterEnumerator : IEnumerator<string>
        {
            public LetterEnumerator()
            {
                Letters = new List<char>()
                {
                    'A'
                };
            }

            public string Current
            {
                get
                {
                    return new(Letters.ToArray());
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                Increment(Letters.Count - 1);
            }

            public bool MoveNext()
            {
                return true;
            }

            public void Reset()
            {
                Letters.Clear();
                Letters.Add('A');
            }

            private IList<char> Letters
            {
                get;
            }

            private void Increment(int position)
            {
                if (position < 0)
                {
                    Letters.Insert(0, 'A');
                }
                else if (Letters[position] == 'Z')
                {
                    Letters[position] = 'A';
                    Increment(position - 1);
                }
                else
                {
                    int asciiValue = Letters[position] + 1;
                    Letters[position] = (char)asciiValue;
                }
            }
        }
    }
}