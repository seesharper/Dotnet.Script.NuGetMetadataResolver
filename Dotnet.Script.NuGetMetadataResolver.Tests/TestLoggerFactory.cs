namespace Microsoft.CodeAnalysis.NuGet.Tests
{
    using System;
    using Extensions.Logging;
    using Xunit.Abstractions;

    public class TestLoggerFactory : ILoggerFactory
    {
        public static ILoggerFactory Instance = new TestLoggerFactory();


        public void Dispose()
        {
            
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger();
        }

        public void AddProvider(ILoggerProvider provider)
        {
        
        }
    }

    public class TestLogger : ILogger, IDisposable
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            ITestOutputHelper testOutputHelper = TestOutputHelper.Current;
            testOutputHelper.WriteLine($"{logLevel} {formatter(state, exception)}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
        }
    }
}