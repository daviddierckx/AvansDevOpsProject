using System;

namespace AvansDevOps.App.Domain.Exceptions
{
    // Custom Exception voor ongeldige state transities of acties
    public class InvalidStateException : InvalidOperationException
    {
        public InvalidStateException() : base("Operation is not valid in the current state.")
        {
        }

        public InvalidStateException(string message) : base(message)
        {
        }

        public InvalidStateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}