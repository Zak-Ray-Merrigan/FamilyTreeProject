using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.Data
{
    public class MissingRequiredAttributeException : KeyNotFoundException
    {
        public MissingRequiredAttributeException(string attribute)
            :base($"{attribute} is missing and must be existent."){}
        
        public MissingRequiredAttributeException(IEnumerable<string> attributes)
            :base($"{string.Join(',', attributes)} are missing and must be existent."){}
    }
}