namespace Dotnet.Script.NuGetMetadataResolver.Logging
{
    using System;

    public static class LogFactory
    {
        private static Func<Type, Action<LogEntry>> factory;

        /// <summary>
        /// Gets or sets the log factory that crates the delegate used for logging.
        /// </summary>
        public static void Initialize(Func<Type, Action<LogEntry>> factory)
        {
            LogFactory.factory = factory;            
        }

        public static Action<LogEntry> GetLogger<T>()
        {
            return factory(typeof(T));
        }
        
    }
}