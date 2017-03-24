namespace Dotnet.Script.NuGetMetadataResolver
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using NuGet.Configuration;
    using NuGet.Frameworks;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    

    /// <summary>
    /// A <see cref="INuGetPackageInstaller"/> that installs packages using the NuGet command    
    /// </summary>
    /// <remarks>
    /// NuGet.PackageManagement is not available on .Net Core so we rely on the NuGet command 
    /// to install packages.
    /// </remarks>
    public class NuGetPackageInstaller : INuGetPackageInstaller
    {
        private readonly ICommandRunner commandRunner;
        private readonly INugetPackageSearcher nugetPackageSearcher;        
        private readonly string rootFolder;
        private readonly ILogger logger;
        private readonly PackagePathResolver packagePathResolver;
        private readonly NuGetFramework netCoreAppFramework;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackageInstaller"/> class.
        /// </summary>
        /// <param name="commandRunner">The <see cref="ICommandRunner"/> that is responsible for 
        /// executing the NuGet command.</param>
        /// <param name="nugetPackageSearcher">The <see cref="INugetPackageSearcher"/> that is responsible for searching for packages.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> that is responsible for creating <see cref="ILogger"/> instances.</param>
        /// <param name="rootFolder">The absolute path to script root folder</param>
        public NuGetPackageInstaller(ICommandRunner commandRunner, INugetPackageSearcher nugetPackageSearcher,ILoggerFactory loggerFactory, string rootFolder)
        {
            netCoreAppFramework = NuGetFramework.Parse(".NETCoreApp,Version=v1.1");
            packagePathResolver = new PackagePathResolver(rootFolder);
            logger = loggerFactory.CreateLogger<NuGetPackageInstaller>();        
            this.commandRunner = commandRunner;
            this.nugetPackageSearcher = nugetPackageSearcher;            
            this.rootFolder = rootFolder;
        }

        /// <inheritdoc />
        public IEnumerable<string> Install(PackageIdentity packageIdentity)
        {
            var defaultSettings = Settings.LoadDefaultSettings(rootFolder);
            NuGetPathContext nugetPathContext = NuGetPathContext.Create(defaultSettings);
            var globalPackagesFolder = nugetPathContext.UserPackageFolder;
            var downloadResourceResult = GlobalPackagesFolderUtility.GetPackage(packageIdentity, globalPackagesFolder);
            if (downloadResourceResult == null)
            {
                logger.LogInformation($"Package {packageIdentity} not found in the global packages folder {globalPackagesFolder}");
                logger.LogInformation($"Installing package {packageIdentity.Id}, Version {packageIdentity.Version}");
                var searchResult = nugetPackageSearcher.Search(packageIdentity);
                if (searchResult == null)
                {
                    return Enumerable.Empty<string>();
                }

                InstallPackage(searchResult);
            }

            downloadResourceResult = GlobalPackagesFolderUtility.GetPackage(packageIdentity, globalPackagesFolder);
            var supportedFrameworks = downloadResourceResult.PackageReader.GetSupportedFrameworks();
            var packageReader = downloadResourceResult.PackageReader;
            var versionFolderPathResolver = new VersionFolderPathResolver(globalPackagesFolder);
            var installPath = versionFolderPathResolver.GetInstallPath(packageIdentity.Id, packageIdentity.Version);
                        
            FrameworkReducer reducer = new FrameworkReducer();
            var nearest = reducer.GetNearest(netCoreAppFramework, supportedFrameworks);

            // Find the nearest dependency set that matches the nearest framework for this package

            //var dependencySetFrameworks = searchResult.PackageSearchMetadata.DependencySets.Select(d => d.TargetFramework);
            //var nearestDependencySetFramework = reducer.GetNearest(nearest, dependencySetFrameworks);
            //var dependencySet =
            //    searchResult.PackageSearchMetadata.DependencySets.Single(
            //        ds => ds.TargetFramework == nearestDependencySetFramework);
            
            
            // dependencies with target framework any is already installed.
            // Still need to figure out the nearest framework for Any dependencies.

            var frameworkSpecificGroup = packageReader.GetLibItems().SingleOrDefault(i => i.TargetFramework == nearest);
            var files = frameworkSpecificGroup.Items.Select(i => i.ToLower()).Where(i => i.EndsWith("dll") && !i.EndsWith("resources.dll"));
            return files.Select(f => Path.GetFullPath(Path.Combine(installPath, f)));
        }

        private void InstallPackage(NuGetPackageSearchResult packageSearchResult)
        {
            var tempPath = Path.GetTempPath();            
            var installDirectory = Path.Combine(tempPath, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(installDirectory);
            var identity = packageSearchResult.PackageSearchMetadata.Identity;
            commandRunner.Execute("nuget",
                $"install {identity.Id} -source {packageSearchResult.Source.Source} -outputdirectory {installDirectory} -version {identity.Version}");
            FolderUtils.Delete(installDirectory);
            
        }








    }
}