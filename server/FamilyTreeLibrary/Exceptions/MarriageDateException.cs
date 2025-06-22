using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Exceptions
{
    public class MarriageDateException : InvalidDateException
    {
        private readonly Family family;
        private readonly FamilyTreeDate date;
        public MarriageDateException(Family f, FamilyTreeDate date)
            :base(date)
        {
            family = f;
            this.date = date;
        }

        public override string Message
        {
            get
            {
                return $"{family.Member.Name} can't be married to {family.InLaw.Name} on {date}, since {family.Member.Name} was born on {family.Member.BirthDate} and {family.InLaw.Name} was born on {family.InLaw.BirthDate}.";
            }
        }
    }
}