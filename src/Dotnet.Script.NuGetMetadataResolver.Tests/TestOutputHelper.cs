namespace Microsoft.CodeAnalysis.NuGet.Tests
{
    using System.Threading;
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
}