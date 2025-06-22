namespace FamilyTreeLibrary.Data
{
    public class UniquenessViolationException : InvalidOperationException
    {
        public UniquenessViolationException() :base() {}

        public UniquenessViolationException(string message) : base(message) {}

        public UniquenessViolationException(string message, Exception innerException) : base(message, innerException) {}
    }
}