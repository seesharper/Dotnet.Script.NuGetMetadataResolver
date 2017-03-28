namespace Dotnet.Script.NuGetMetadataResolver.Logging
{
    using System;

    /// <summary>
    /// A simple static logging factory. 
    /// </summary>
    public static class LogFactory
    {
        private static Func<Type, Action<LogEntry>> factory;

        static LogFactory()
        {
            factory = type => (entry => Console.WriteLine($"{entry.Level} {entry.Message} {entry.Exception}"));
        }

        /// <summary>
        /// Gets or sets the log factory that crates the delegate used for logging.
        /// </summary>
        public static void Initialize(Func<Type, Action<LogEntry>> valueFactory)
        {
            factory = valueFactory;            
        }

        /// <summary>
        /// Gets a log action used to log messages from the type identified by <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> for which to create a log action.</typeparam>
        /// <returns>An action that represents logging messages.</returns>
        public static Action<LogEntry> GetLogger<T>()
        {
            return factory(typeof(T));
        }
        
    }
}