namespace Dotnet.Script.NuGetMetadataResolver.Logging
{
    using System;

    /// <summary>
    /// Extends the log delegate to simplify creating log entries.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Logs a new entry with the <see cref="LogLevel.Info"/> level.
        /// </summary>
        /// <param name="logAction">The log delegate.</param>
        /// <param name="message">The message to be logged.</param>
        public static void Info(this Action<LogEntry> logAction, string message)
        {
            logAction(new LogEntry(LogLevel.Info, message));
        }

        /// <summary>
        /// Logs a new entry with the <see cref="LogLevel.Debug"/> level.
        /// </summary>
        /// <param name="logAction">The log delegate.</param>
        /// <param name="message">The message to be logged.</param>
        public static void Debug(this Action<LogEntry> logAction, string message)
        {
            logAction(new LogEntry(LogLevel.Info, message));
        }

        /// <summary>
        /// Logs a new entry with the <see cref="LogLevel.Warning"/> level.
        /// </summary>
        /// <param name="logAction">The log delegate.</param>
        /// <param name="message">The message to be logged.</param>
        public static void Warning(this Action<LogEntry> logAction, string message)
        {
            logAction(new LogEntry(LogLevel.Warning, message));
        }

        /// <summary>
        /// Logs a new entry with the <see cref="LogLevel.Error"/> level.
        /// </summary>
        /// <param name="logAction">The log delegate.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The <see cref="Exception"/> causing the error.</param>
        public static void Error(this Action<LogEntry> logAction, string message, Exception exception = null)
        {
            logAction(new LogEntry(LogLevel.Error, message, exception));
        }
    }
}