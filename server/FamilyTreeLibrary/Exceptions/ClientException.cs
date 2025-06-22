using System.Collections;
using System.Net;

namespace FamilyTreeLibrary.Exceptions
{
    public abstract class ClientException : Exception
    {
        protected ClientException(string message)
            :base(message){}
        
        protected ClientException(string message, Exception cause)
            :base(message, cause) {}

        public override IDictionary Data
        {
            get
            {
                return new Dictionary<string,object>()
                {
                    {"Message", Message},
                    {"Status Code", StatusCode}
                };
            }
        }

        public abstract HttpStatusCode StatusCode
        {
            get;
        }

        public override string ToString()
        {
            return $"Status {Data["Status Code"]}: {Data["Message"]}";
        }
    }
}