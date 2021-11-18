using System;

namespace AeroSharp.Serialization
{
    /// <summary>
    /// A class for wrapping serialization exceptions.
    /// </summary>
    public class SerializationException : Exception
    {
        /// <summary>
        /// Create a new instance of the <see cref="SerializationException"/> class.
        /// </summary>
        public SerializationException()
        {
        }

        /// <summary>
        /// Create a new instance of the <see cref="SerializationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public SerializationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Create a new instance of the <see cref="SerializationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner <see cref="Exception"/>.</param>
        public SerializationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
