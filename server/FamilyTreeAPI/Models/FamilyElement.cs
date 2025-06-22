using FamilyTreeLibrary.Exceptions;

namespace FamilyTreeAPI.Models
{
    public class FamilyElement : IEquatable<FamilyElement>
    {
        public PersonElement Member
        {
            get;
            set;
        }

        public PersonElement InLaw
        {
            get;
            set;
        }

        public string MarriageDate
        {
            get;
            set;
        }

        public bool Equals(FamilyElement other)
        {
            try
            {
                return Member == other.Member && InLaw == other.InLaw && MarriageDate == other.MarriageDate;
            }
            catch (NullReferenceException ex)
            {
                throw new ClientBadRequestException("A family element can't be null.", ex);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is FamilyElement other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Member, InLaw, MarriageDate);
        }

        public override string ToString()
        {
            string memberMessage = $"Member = ({Member})";
            string inLawMessage = "InLaw = " + (InLaw is null ? "unknown" : $"({InLaw})");
            string marriageDateMessage = "Marriage Date = " + (MarriageDate is null ? "unknown" : MarriageDate);
            return $"{memberMessage}; {inLawMessage}; {marriageDateMessage};";
        }

        public static bool operator== (FamilyElement a, FamilyElement b)
        {
            try
            {
                return a.Equals(b);
            }
            catch (NullReferenceException ex)
            {
                throw new ClientBadRequestException("A family element can't be null.", ex);
            }
        }

        public static bool operator!= (FamilyElement a, FamilyElement b)
        {
            try
            {
                return !a.Equals(b);
            }
            catch (NullReferenceException ex)
            {
                throw new ClientBadRequestException("A Family element can't be null.", ex);
            }
        }
    }
}