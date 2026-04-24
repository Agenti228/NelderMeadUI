using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexUI.Exceptions
{
    internal class InvalidDigitInputException : Exception
    {
        public InvalidDigitInputException()
        {
        }

        public InvalidDigitInputException(string message) : base(message)
        {
        }

        public InvalidDigitInputException(string message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
