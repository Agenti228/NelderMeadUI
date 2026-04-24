using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexUI.Exceptions
{
    internal class EmptyExpressionException : Exception
    {
        public EmptyExpressionException()
        {
        }

        public EmptyExpressionException(string message) : base(message)
        {
        }

        public EmptyExpressionException(string message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
