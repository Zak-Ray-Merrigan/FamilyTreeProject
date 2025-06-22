namespace FamilyTreeLibrary.Data
{
    public class UniqueIdentifierNotExistsException : KeyNotFoundException
    {
        public UniqueIdentifierNotExistsException(string message)
            :base(message){}
        
        public UniqueIdentifierNotExistsException(string message, Exception innerException)
            : base(message, innerException){}
    }
}