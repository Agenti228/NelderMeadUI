using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexUI.Exceptions
{
    internal class InvalidOperatorLocationException : Exception
    {
        public InvalidOperatorLocationException()
        {
        }

        public InvalidOperatorLocationException(string message) : base(message)
        {
        }

        public InvalidOperatorLocationException(string message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
