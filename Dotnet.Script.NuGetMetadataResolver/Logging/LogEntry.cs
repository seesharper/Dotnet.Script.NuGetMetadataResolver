namespace Dotnet.Script.NuGetMetadataResolver.Logging
{
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
        public LogEntry(LogLevel level, string message)
        {
            Level = level;
            Message = message;
        }

        /// <summary>
        /// Gets the <see cref="LogLevel"/> for this entry.
        /// </summary>
        public LogLevel Level { get; private set; }

        /// <summary>
        /// Gets the log message for this entry.
        /// </summary>
        public string Message { get; private set; }
    }
}