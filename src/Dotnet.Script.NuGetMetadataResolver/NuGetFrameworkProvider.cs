namespace Dotnet.Script.NuGetMetadataResolver
{
    using System.Linq;
    using System.Reflection;
    using NuGet.Frameworks;

    public class NugetFrameworkProvider
    {        
        public static NuGetFramework ParseFrameworkName(string frameworkName = null)
        {
            return frameworkName == null
                            ? NuGetFramework.AnyFramework
                            : NuGetFramework.ParseFrameworkName(frameworkName, new DefaultFrameworkNameProvider());
        }

        public static NuGetFramework GetFrameworkNameFromAssembly()
        {         
            return ParseFrameworkName(Assembly.GetEntryAssembly().GetCustomAttributes()
                            .OfType<System.Runtime.Versioning.TargetFrameworkAttribute>()
                            .Select(x => x.FrameworkName)
                            .FirstOrDefault());
        }
    }
}