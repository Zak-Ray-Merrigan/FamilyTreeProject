using FamilyTreeLibrary.Exceptions;

namespace FamilyTreeLibrary.Models
{
    public partial struct FamilyTreeDate : ICloneable, IComparable<FamilyTreeDate>, IEquatable<FamilyTreeDate>
    {
        private int day;
        private string month;
        private string year;
        private IReadOnlyDictionary<string,int> months;
        public FamilyTreeDate(){}
        public FamilyTreeDate(string input)
        {
            month = DefaultDate.Month;
            day = DefaultDate.Day;
            year = DefaultDate.Year;
            if (input is not null)
            {
                string[] values = input.Split(' ');
                if (values.Length < 4)
                {
                    foreach (string value in values)
                    {
                        if (value.Length < 3)
                        {
                            day = value == "" ? 0 : Convert.ToInt32(value);
                        }
                        else if (value.Length == 3 || value == "Jan.")
                        {
                            month = value;
                        }
                        else
                        {
                            year = value;
                        }
                    }
                }
            }
            Year = year;
            Month = month;
            Day = day;
        }

        public static FamilyTreeDate DefaultDate
        {
            get
            {
                return new()
                {
                    Year = "",
                    Month = "",
                    Day = 0
                };
            }
        }

        public int Day
        {
            readonly get
            {
                return day;
            }
            set
            {
                if (value < 0 || value > months[Month])
                {
                    throw new InvalidDateException(value, DateAttributes.Day);
                }
                day = value;
            }
        }

        public string Month
        {
            readonly get
            {
                return month;
            }
            set
            {
                if(!Months.ContainsKey(value))
                {
                    throw new InvalidDateException(value, DateAttributes.Month);
                }
                month = value;
            }
        }

        public string Year
        {
            readonly get
            {
                return year;
            }
            set
            {
                bool isNumber = FamilyTreeUtils.NumberPattern().IsMatch(value);
                if (value != "" && !isNumber && !FamilyTreeUtils.RangePattern().IsMatch(value))
                {
                    throw new InvalidDateException(value, DateAttributes.Year);
                }
                year = value;
                FillMonths(isNumber && DateTime.IsLeapYear(Convert.ToInt32(year)));
            }
        }

        public readonly object Clone()
        {
            return new FamilyTreeDate(ToString());
        }

        public readonly int CompareTo(FamilyTreeDate other)
        {
            if (IsDefault(other))
            {
                return 1;
            }
            int yearDiff = Year.CompareTo(other.Year);
            if (yearDiff != 0)
            {
                return yearDiff;
            }
            int monthDiff = Math.Abs(months.Keys.ToList().IndexOf(Month) - months.Keys.ToList().IndexOf(other.Month));
            return monthDiff != 0 ? monthDiff : Math.Abs(Day - other.Day);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is FamilyTreeDate other && Equals(other);
        }

        public readonly bool Equals(FamilyTreeDate other)
        {
            return CompareTo(other) == 0;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Day, Month, Year);
        }

        public override readonly string ToString()
        {
            string output = "";
            if (Day > 0)
            {
                output += Day;
            }
            if (Month is not null && Month != "")
            {
                output += $" {Month} ";
            }
            if (Year is not null && Year != "")
            {
                output += Year;
            }
            string result = output.Trim();
            return result == "" ? null : result;
        }

        public static bool IsDefault(FamilyTreeDate date)
        {
            return date.Day == 0 && date.Month is null && date.Year is null && date.Months is null;
        }

        public static bool operator==(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            bool dateAIsDefault = IsDefault(dateA);
            bool dateBIsDefault = IsDefault(dateB);
            return (dateAIsDefault && dateBIsDefault) || (!dateAIsDefault && dateA.Equals(dateB));
        }

        public static bool operator!=(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            bool dateAIsDefault = IsDefault(dateA);
            bool dateBIsDefault = IsDefault(dateB);
            return (!dateAIsDefault || !dateBIsDefault) && (dateAIsDefault || !dateA.Equals(dateB));
        }

        public static bool operator<(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            bool dateAIsDefault = IsDefault(dateA);
            bool dateBIsDefault = IsDefault(dateB);
            return (dateAIsDefault && !dateBIsDefault) || (!dateBIsDefault && dateA.CompareTo(dateB) < 0);
        }

        public static bool operator>(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            bool dateAIsDefault = IsDefault(dateA);
            bool dateBIsDefault = IsDefault(dateB);
            return (!dateAIsDefault && dateBIsDefault) || (!dateAIsDefault && dateA.CompareTo(dateB) > 0);
        }

        internal readonly IReadOnlyDictionary<string,int> Months
        {
            get
            {
                return months;
            }
        }

        private void FillMonths(bool isLeapYear)
        {
            months = new Dictionary<string,int>
                {
                    {"", 0},
                    {"Jan.", 31},
                    {"Feb",isLeapYear ? 29 : 28},
                    {"Mar",31},
                    {"Apr",30},
                    {"May",31},
                    {"Jun",30},
                    {"Jul",31},
                    {"Aug",31},
                    {"Sep",30},
                    {"Oct",31},
                    {"Nov",30},
                    {"Dec",31}
                };
        }
    }
}