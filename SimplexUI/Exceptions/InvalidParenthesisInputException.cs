using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexUI.Exceptions
{
    internal class InvalidParenthesisInputException : Exception
    {
        public InvalidParenthesisInputException()
        {
        }

        public InvalidParenthesisInputException(string message) : base(message)
        {
        }

        public InvalidParenthesisInputException(string message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
