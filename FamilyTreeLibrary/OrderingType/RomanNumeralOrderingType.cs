using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FamilyTreeLibrary.Exceptions;

namespace FamilyTreeLibrary.OrderingType
{
    public class RomanNumeralOrderingType : AbstractOrderingType
    {
        internal RomanNumeralOrderingType(int key, bool isUpperCase)
        :base(key, isUpperCase ? 1 : 6)
        {
        }

        internal RomanNumeralOrderingType(string value, bool isUpperCase)
        :base(value, isUpperCase ? 1 : 6)
        {
        }

        protected override int FindKey(string value)
        {
            string pattern;
            switch (Type)
            {
                case OrderingTypeTypes.RomanNumeralUpper: pattern = @"^[MDCLXVI]+\.$"; break;
                case OrderingTypeTypes.RomanNumeralLower: pattern = @"^[mdclxvi]+\)$"; break;
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
            }
            return key;
        }

        protected override string FindValue(int key)
        {
            if (key < 0)
            {
                return "";
            }
            string result = "";
            int number = key;
            foreach (KeyValuePair<int,string> item in SpecialConversionPairs)
            {
                while (number >= item.Key)
                {
                    result += item.Value;
                    number -= item.Key;
                }
            }
            switch (Type)
            {
                case OrderingTypeTypes.RomanNumeralUpper: result += "."; break;
                case OrderingTypeTypes.RomanNumeralLower: result = $"{result.ToLower()})"; break;
                default: return "";
            }
            return result;
        }

        private static IReadOnlyDictionary<int,string> SpecialConversionPairs
        {
            get
            {
                return new Dictionary<int,string>()
                {
                    { 1000, "M" },
                    { 900, "CM" },
                    { 500, "D" },
                    { 400, "CD" },
                    { 100, "C" },
                    { 90, "XC" },
                    { 50, "L" },
                    { 40, "XL" },
                    { 10, "X" },
                    { 9, "IX" },
                    { 5, "V" },
                    { 4, "IV" },
                    { 1, "I" }
                };
            }
        }
    }
}