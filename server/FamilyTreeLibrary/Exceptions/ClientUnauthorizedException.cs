using System.Net;

namespace FamilyTreeLibrary.Exceptions
{
    public class ClientUnauthorizedException : ClientException
    {
        public ClientUnauthorizedException(string message)
            :base(message) {}
        
        public ClientUnauthorizedException(string message, Exception cause)
            :base(message, cause) {}

        public override HttpStatusCode StatusCode
        {
            get
            {
                return HttpStatusCode.Unauthorized;
            }
        }
    }
}