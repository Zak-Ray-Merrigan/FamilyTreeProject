using System.Net;

namespace FamilyTreeLibrary.Exceptions
{
    public class ClientForbiddenException : ClientException
    {
        public ClientForbiddenException(string message)
            :base(message) {}
        
        public ClientForbiddenException(string message, Exception cause)
            :base(message, cause) {}

        public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
    }
}