using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Exceptions
{
    public class InvalidDateException : FormatException
    {
        private readonly DateAttributes attributes;
        private readonly object input;

        public InvalidDateException(object input, DateAttributes attributes = DateAttributes.General)
            :base()
        {
            this.attributes = attributes;
            this.input = input;
        }

        public InvalidDateException(object input, Exception innerException, DateAttributes attributes = DateAttributes.General)
            :base(input.ToString(), innerException)
        {
            this.attributes = attributes;
            this.input = input;
        }

        public override string Message
        {
            get
            {
                return attributes switch
                {
                    DateAttributes.Day => $"{input} is an invalid day.",
                    DateAttributes.Month => $"{input} isn't a month",
                    DateAttributes.Year => $"{input} isn't a year",
                    _ => $"{input} isn't a valid date."
                };
            }
        }
    }
}