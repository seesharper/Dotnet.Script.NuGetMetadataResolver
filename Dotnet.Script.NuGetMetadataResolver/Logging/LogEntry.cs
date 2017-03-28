namespace Dotnet.Script.NuGetMetadataResolver.Logging
{
    using System;
    using System.Runtime.CompilerServices;

    

    
    /// <summary>
    /// Represents a log entry.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="level">The <see cref="LogLevel"/> of this entry.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the entry.</param>
        public LogEntry(LogLevel level, string message, Exception exception = null)
        {                        
            Level = level;
            Message = message;
            Exception = exception;
        }

        /// <summary>
        /// Gets the <see cref="LogLevel"/> for this entry.
        /// </summary>
        public LogLevel Level { get; private set; }

        /// <summary>
        /// Gets the log message for this entry.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the <see cref="Exception"/> associated with this entry.
        /// </summary>
        public Exception Exception { get; private set; }
    }
}