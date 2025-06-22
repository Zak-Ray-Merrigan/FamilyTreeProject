using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary
{
    public class InvalidDateException : FormatException
    {
        public InvalidDateException(string message) : base(message) {}
        public InvalidDateException(string message, Exception innerException) : base(message, innerException){}
    }
}