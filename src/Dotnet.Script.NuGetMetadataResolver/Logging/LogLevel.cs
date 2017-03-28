namespace Dotnet.Script.NuGetMetadataResolver.Logging
{
    /// <summary>
    /// Describes the logging level/severity.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Indicates the <see cref="LogEntry"/> represents an information message.
        /// </summary>
        Info,

        /// <summary>
        /// Indicates the <see cref="LogEntry"/> represents a warning message.
        /// </summary>
        Debug,

        /// <summary>
        /// Indicates the <see cref="LogEntry"/> represents a warning message.
        /// </summary>
        Warning,

        /// <summary>
        /// Indicates the <see cref="LogEntry"/> represents an error message.
        /// </summary>
        Error
    }
}