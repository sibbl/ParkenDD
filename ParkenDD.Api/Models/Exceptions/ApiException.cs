using System;

namespace ParkenDD.Api.Models.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException()
            : base()
        {
        }
        public ApiException(string message)
            : base(message)
        {
        }

        public ApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
