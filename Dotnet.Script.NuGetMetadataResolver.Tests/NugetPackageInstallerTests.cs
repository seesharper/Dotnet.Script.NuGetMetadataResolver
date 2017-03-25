namespace Dotnet.Script.NuGetMetadataResolver.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Logging;
    using Microsoft.CodeAnalysis.NuGet.Tests;
    using NuGet.Packaging.Core;
    using NuGet.Versioning;
    using NuGetMetadataResolver;
    using Xunit;
    using Xunit.Abstractions;

    public class NugetPackageInstallerTests
    {
        public NugetPackageInstallerTests(ITestOutputHelper testOutputHelper)
        {
            LogFactory.Initialize(type => (entry => testOutputHelper.WriteLine($"{entry.Level} {entry.Message}")));            
            testOutputHelper.Capture();
        }


        [Fact]
        public void ShouldInstallPackage()
        {
            //string directory = Path.GetDirectoryName(new Uri(typeof(NuGetMetadataReferenceResolver).GetTypeInfo().Assembly.CodeBase).LocalPath);
                                   
            //NuGetPackageInstaller installer = new NuGetPackageInstaller(new CommandRunner(), 
            //    new NuGetPackageSearcher(
            //        new NuGetPackageSourceProvider(Directory.GetCurrentDirectory())
            //        , Path.Combine(directory, "packages"));

            //installer.Install(new PackageIdentity("LightInject", NuGetVersion.Parse("5.0.0")));
        }
        
    }
}
