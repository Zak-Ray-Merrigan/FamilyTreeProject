using FamilyTreeLibrary.Exceptions;

namespace FamilyTreeAPI.Models
{
    public class PersonElement : IEquatable<PersonElement>
    {
        public string Name
        {
            get;
            set;
        }

        public string BirthDate
        {
            get;
            set;
        }

        public string DeceasedDate
        {
            get;
            set;
        }

        public bool Equals(PersonElement other)
        {
            try
            {
                return Name == other.Name && BirthDate == other.BirthDate && DeceasedDate == other.DeceasedDate;
            }
            catch (NullReferenceException ex)
            {
                throw new ClientBadRequestException("Person Element can't be null.", ex);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is PersonElement other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, BirthDate, DeceasedDate);
        }

        public override string ToString()
        {
            string nameMessage = "Name = " + (Name is null ? "unkown" : Name);
            string birthDateMessage = "Birth Date = " + (BirthDate is null ? "unkown" : BirthDate);
            string deceasedDateMessage = "Deceased Date = " + (DeceasedDate is null ? "unkown" : DeceasedDate);
            return $"{nameMessage}; {birthDateMessage}; {deceasedDateMessage};";
        }

        public static bool operator== (PersonElement a, PersonElement b)
        {
            try
            {
                return a.Equals(b);
            }
            catch (NullReferenceException ex)
            {
                throw new ClientBadRequestException("Person element can't be null.", ex);
            }
        }

        public static bool operator!= (PersonElement a, PersonElement b)
        {
            try
            {
                return !a.Equals(b);
            }
            catch (NullReferenceException ex)
            {
                throw new ClientBadRequestException("Person element can't be null.", ex);
            }
        }
    }
}