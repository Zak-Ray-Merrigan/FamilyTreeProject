using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary
{
    public class InvalidAttributeException : InvalidOperationException
    {
        public InvalidAttributeException(string attribute)
            :base($"{attribute} is required; meaning that, it's not an extended attribute."){}
        public InvalidAttributeException(IEnumerable<string> attributes)
            :base($"{string.Join(',', attributes)} are required; meaning that, these attributes aren't extended attributes."){}
    }
}