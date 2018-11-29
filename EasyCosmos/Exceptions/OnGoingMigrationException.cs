using System;

namespace EasyCosmos.Exceptions
{
    public class OnGoingMigrationException : Exception
    {
        public OnGoingMigrationException(string message):base(message)
        {
            
        }
    }
}