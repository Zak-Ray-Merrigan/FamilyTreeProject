using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Exceptions
{
    public class DeceasedDateException : InvalidDateException
    {
        private readonly string name;
        private readonly FamilyTreeDate birthDate;
        private readonly FamilyTreeDate date;
        public DeceasedDateException(Person p, FamilyTreeDate date)
        :base(date) 
        {
            name = p.Name;
            birthDate = p.BirthDate;
            this.date = date;
        }

        public override string Message
        {
            get
            {
                return $"{name} can't be deceased on {date}, since {name} was born on {birthDate}.";
            }
        }


    }
}