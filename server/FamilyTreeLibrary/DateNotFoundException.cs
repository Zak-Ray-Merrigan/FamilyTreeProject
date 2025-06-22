using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary
{
    public class DateNotFoundException : ArgumentException
    {
        public DateNotFoundException(string message) : base(message) {}

        public DateNotFoundException(string message, Exception innerException) : base(message, innerException){}
    }
}