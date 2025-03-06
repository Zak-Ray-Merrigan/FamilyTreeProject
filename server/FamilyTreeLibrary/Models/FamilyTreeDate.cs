using FamilyTreeLibrary.Serialization;
using System.Text.RegularExpressions;

namespace FamilyTreeLibrary.Models
{
    public partial class FamilyTreeDate : AbstractComparableBridge, IComparable<FamilyTreeDate>, IEquatable<FamilyTreeDate>
    {
        private readonly string day;
        private readonly string month;
        private readonly string year;
        private readonly static IReadOnlySet<string> MONTHS = new SortedSet<string>(new MonthComparer()) {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};

        public FamilyTreeDate(string input)
        {
            string value = input.Trim();
            string[] parts = value.Split();
            switch (parts.Length)
            {
                case 0: day = ""; month = ""; year = ""; break;
                case 1:
                    day = "";
                    month = "";
                    year = InitializeYear(parts[0]);
                    break;
                case 2:
                    day = "";
                    month = InitializeMonth(parts[0]);
                    year = InitializeYear(parts[1]);
                    break;
                case 3:
                    day = InitializeDay(parts[0]);
                    month = InitializeMonth(parts[1]);
                    year = InitializeYear(parts[2]);
                    break;
                default: throw new DateNotFoundException($"{value} isn't a date.");
            }
        }

        public override BridgeInstance Instance
        {
            get
            {
                if (day == "" && month == "" && year == "")
                {
                    return new();
                }
                else if (day == "" && month == "")
                {
                    return new(year);
                }
                else if (day == "")
                {
                    return new($"{month} {year}");
                }
                return new($"{day} {month} {year}");
            }
        }

        public override int CompareTo(AbstractComparableBridge? other)
        {
            return CompareTo(other as FamilyTreeDate);
        }

        public int CompareTo(FamilyTreeDate? other)
        {
            if (other is null)
            {
                return 1;
            }
            IReadOnlyList<int> yearPartsA = [.. year.Split('-').Select((v1) => Convert.ToInt32(v1))];
            IReadOnlyList<int> yearPartsB = [.. other.year.Split('-').Select((v2) => Convert.ToInt32(v2))];
            bool PartsAIsRange = yearPartsA.Count > 1;
            bool PartsBIsRange = yearPartsB.Count > 1;
            int yearCompare;
            if (PartsAIsRange && PartsBIsRange)
            {
                yearCompare = yearPartsA[0] == yearPartsB[0] ? yearPartsA[1] - yearPartsB[1] : yearPartsA[0] - yearPartsB[0];
            }
            else if (!PartsAIsRange && PartsBIsRange)
            {
                yearCompare = yearPartsA[0] > yearPartsB[0] ? (yearPartsA[0] <= yearPartsB[1] ? 0 : yearPartsA[0] - yearPartsB[1]) : yearPartsA[0] - yearPartsB[0];
            }
            else if (PartsAIsRange && !PartsBIsRange)
            {
                yearCompare = yearPartsA[0] < yearPartsB[0] ? (yearPartsA[1] >= yearPartsB[0] ? 0 : yearPartsA[1] - yearPartsB[0]) : yearPartsA[0] - yearPartsB[0];
            }
            else
            {
                yearCompare = yearPartsA[0] - yearPartsB[0];
            }
            if (yearCompare != 0)
            {
                return yearCompare;
            }
            IComparer<string> monthCompare = new MonthComparer();
            int monthCompareResult = monthCompare.Compare(month, other.month);
            if (monthCompareResult != 0)
            {
                return monthCompareResult;
            }
            return Convert.ToInt32(day) - Convert.ToInt32(other.day);
        }

        public bool Equals(FamilyTreeDate? other)
        {
            return base.Equals(other);
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private static string InitializeDay(string token)
        {
            if (DayPattern().Matches(token).Count != 1)
            {
                throw new InvalidDateException($"{token} isn't a valid day.");
            }
            return token;
        }

        private static string InitializeMonth(string token)
        {
            if (!MONTHS.Contains(token))
            {
                throw new InvalidDateException($"{token} isn't a month.");
            }
            return token;
        }

        private static string InitializeYear(string token)
        {
            if (YearPattern().Matches(token).Count != 1)
            {
                throw new InvalidDateException($"{token} isn't a year.");
            }
            return token;
        }

        [GeneratedRegex(@"^[1-9]\d{3}|[1-9]\d{3}-[1-9]\d{3}$", RegexOptions.Compiled)]
        private static partial Regex YearPattern();
        [GeneratedRegex(@"^\d{2}|\d$", RegexOptions.Compiled)]
        private static partial Regex DayPattern();

        private class MonthComparer : IComparer<string>, IEqualityComparer<string>
        {
            public int Compare(string? x, string? y)
            {
                return GetHashCode(x!) - GetHashCode(y!);
            }

            public bool Equals(string? x, string? y)
            {
                return Compare(x, y) == 0;
            }
            public int GetHashCode(string obj)
            {
                switch (obj)
                {
                    case "Jan": return 1;
                    case "Feb": return 2;
                    case "Mar": return 3;
                    case "Apr": return 4;
                    case "May": return 5;
                    case "Jun": return 6;
                    case "Jul": return 7;
                    case "Aug": return 8;
                    case "Sep": return 9;
                    case "Oct": return 10;
                    case "Nov": return 11;
                    case "Dec": return 12;
                    default: return 0;
                }
            }
        }
    }
}