using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexUI.Exceptions
{
    internal class UndefinedParseException : Exception
    {
        public UndefinedParseException()
        {
        }

        public UndefinedParseException(string message) : base(message)
        {
        }

        public UndefinedParseException(string message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
