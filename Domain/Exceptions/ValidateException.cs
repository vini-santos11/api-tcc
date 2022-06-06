using System;

namespace Domain.Exceptions
{
    public class ValidateException : Exception
    {
        public ValidateException(string message) : base(message)
        {

        }
    }
}
