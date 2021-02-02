using System;
using System.Runtime.Serialization;

namespace Atata.WebDriverSetup
{
    [Serializable]
    public class DriverSetupException : Exception
    {
        public DriverSetupException()
        {
        }

        public DriverSetupException(string message)
            : base(message)
        {
        }

        public DriverSetupException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DriverSetupException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
