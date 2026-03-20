namespace SimplexUI.Exceptions
{
    public class UnsortedArrayException : Exception
    {
        public UnsortedArrayException()
        {
        }

        public UnsortedArrayException(string message) : base(message)
        {
        }

        public UnsortedArrayException(string message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
