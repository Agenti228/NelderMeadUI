namespace SimplexUI.Exceptions
{
    public class MismatchingDimentionsException : Exception
    {
        public MismatchingDimentionsException()
        {
        }

        public MismatchingDimentionsException(string message) : base(message)
        {
        }

        public MismatchingDimentionsException(string message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
