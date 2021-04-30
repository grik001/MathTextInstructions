using System;
using System.Collections.Generic;
using System.Text;

namespace MathInstructionProcessor.Common.CustomExceptions
{
    [Serializable()]
    public class InvalidInstructionException : System.Exception
    {
        public InvalidInstructionException() : base() { }
        public InvalidInstructionException(string message) : base(message) { }
        public InvalidInstructionException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected InvalidInstructionException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
