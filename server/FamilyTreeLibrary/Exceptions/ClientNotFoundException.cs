using System.Net;

namespace FamilyTreeLibrary.Exceptions
{
    public class ClientNotFoundException : ClientException
    {
        public ClientNotFoundException(string message)
            :base(message) {}

        public ClientNotFoundException(string message, Exception cause)
            :base(message, cause) {}

        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    }
}