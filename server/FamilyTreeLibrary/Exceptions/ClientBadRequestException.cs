using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.Exceptions
{
    public class ClientBadRequestException : ClientException
    {
        public ClientBadRequestException(string message)
            :base(message) {}
        
        public ClientBadRequestException(string message, Exception cause)
            :base(message, cause) {}

        public override HttpStatusCode StatusCode
        {
            get
            {
                return HttpStatusCode.BadRequest;
            }
        }
    }
}