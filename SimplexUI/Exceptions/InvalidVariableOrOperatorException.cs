using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexUI.Exceptions
{
    internal class InvalidVariableOrOperatorException : Exception
    {
        public InvalidVariableOrOperatorException()
        {
        }

        public InvalidVariableOrOperatorException(string message) : base(message)
        {
        }

        public InvalidVariableOrOperatorException(string message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
