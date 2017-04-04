namespace Microsoft.CodeAnalysis.NuGet.Tests
{
    using System;
    using System.Threading;
    using Extensions.Logging;
    using Xunit.Abstractions;

    public static class TestOutputHelper
    {
        private static readonly AsyncLocal<ITestOutputHelper> CurrentTestOutputHelper
            = new AsyncLocal<ITestOutputHelper>();

        public static void Capture(this ITestOutputHelper outputHelper)
        {
            CurrentTestOutputHelper.Value = outputHelper;
        }

        public static ITestOutputHelper Current => CurrentTestOutputHelper.Value;
    }

    public class TestOutputLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            TestOutputHelper.Current.WriteLine($"{logLevel} {formatter(state, exception)}");                        
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }

    public class TestOutPutLoggerFactory : ILoggerFactory
    {
        public void Dispose()
        {
            
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestOutputLogger();
        }

        public void AddProvider(ILoggerProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}