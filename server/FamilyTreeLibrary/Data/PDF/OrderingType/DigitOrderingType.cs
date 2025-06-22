namespace FamilyTreeLibrary.Data.PDF.OrderingType
{
    public partial class DigitOrderingType : AbstractOrderingType
    {
        internal DigitOrderingType(int key, bool isParenthesized, int maxKey = int.MaxValue)
        :base(key, !isParenthesized ? 3 : 5, maxKey)
        {
        }

        internal DigitOrderingType(string value, bool isParenthesized, int maxKey = int.MaxValue)
        :base(value, !isParenthesized ? 3 : 5, maxKey)
        {
        }

        protected override int FindKey(string value)
        {
            if (Type != OrderingTypeTypes.Numbering)
            {
                return 0;
            }
            string v = value.Length > 0 ? value[..(value.Length - 1)] : value;
            if (FamilyTreeUtils.NumberPattern().IsMatch(v))
            {
                int key = Convert.ToInt32(v);
                return key <= MaxKey ? key : 0;
            }
            return 0;
        }

        protected override string FindValue(int key)
        {
            return Type == OrderingTypeTypes.Numbering && key > 0 && key <= MaxKey? $"{key}." : "";
        }
    }
}