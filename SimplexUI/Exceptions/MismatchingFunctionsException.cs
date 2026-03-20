namespace SimplexUI.Exceptions
{
    public class MismatchingFunctionsException : Exception
    {
        public MismatchingFunctionsException()
        {
        }

        public MismatchingFunctionsException(string message) : base(message)
        {
        }

        public MismatchingFunctionsException(string message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
