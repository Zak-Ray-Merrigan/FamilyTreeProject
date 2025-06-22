using System.Text.RegularExpressions;

namespace FamilyTreeLibrary.Data.PDF.OrderingType
{
    public partial class ParenthesizedDigitOrderingType : DigitOrderingType
    {
        internal ParenthesizedDigitOrderingType(int key, int maxKey = int.MaxValue)
        :base(key, true, maxKey)
        {
        }

        internal ParenthesizedDigitOrderingType(string value, int maxKey = int.MaxValue)
        :base(value, true, maxKey)
        {
        }

        protected override int FindKey(string value)
        {
            string v = MyRegex1().IsMatch(value) ? value[1..^1] : "";
            if (Type == OrderingTypeTypes.ParenthesizedNumbering && FamilyTreeUtils.NumberPattern().IsMatch(v))
            {
                int key = Convert.ToInt32(v);
                return key <= MaxKey ? key : 0;
            }
            return 0;
        }

        protected override string FindValue(int key)
        {
            return Type == OrderingTypeTypes.ParenthesizedNumbering && key > 0 && key <= MaxKey ? $"({key})" : "";
        }

        [GeneratedRegex("^\\(\\d+\\)$")]
        private static partial Regex MyRegex1();
    }
}