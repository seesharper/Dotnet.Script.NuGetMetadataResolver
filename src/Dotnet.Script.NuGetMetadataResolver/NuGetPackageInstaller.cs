namespace Dotnet.Script.NuGetMetadataResolver
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Logging;
    using Microsoft.Extensions.DependencyModel;
    using NuGet.Configuration;
    using NuGet.Frameworks;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.Protocol;

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
        private readonly NuGetFramework framework;
        private readonly string rootFolder;
        private readonly Action<LogEntry> logger = LogFactory.GetLogger<NuGetPackageInstaller>();


        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackageInstaller"/> class.
        /// </summary>
        /// <param name="commandRunner">The <see cref="ICommandRunner"/> that is responsible for 
        /// executing the NuGet command.</param>
        /// <param name="nugetPackageSearcher">The <see cref="INugetPackageSearcher"/> that is responsible for searching for packages.</param>
        /// <param name="framework">The <see cref="NuGetFramework"/> that indicates the context for which to install packages.</param>
        /// <param name="rootFolder">The absolute path to script root folder</param>
        public NuGetPackageInstaller(ICommandRunner commandRunner, INugetPackageSearcher nugetPackageSearcher, NuGetFramework framework,string rootFolder)
        {            
            this.commandRunner = commandRunner;
            this.nugetPackageSearcher = nugetPackageSearcher;            
            this.framework = new FallbackFramework(framework, new NuGetFramework[] { NuGetFramework.Parse("dotnet") });
            this.rootFolder = rootFolder;                        
        }

        /// <inheritdoc />
        public void Install(Dictionary<PackageIdentity, IEnumerable<string>> referencedPackages, PackageIdentity packageIdentity)
        {
            logger.Info($"Resolving package {packageIdentity} in the context of {framework}.");
            if (referencedPackages.ContainsKey(packageIdentity))
            {
                logger.Info($"Package {packageIdentity} already installed and referenced");
                return;
            }

            var defaultSettings = Settings.LoadDefaultSettings(rootFolder);
            NuGetPathContext nugetPathContext = NuGetPathContext.Create(defaultSettings);
            var globalPackagesFolder = nugetPathContext.UserPackageFolder;
            var downloadResourceResult = GlobalPackagesFolderUtility.GetPackage(packageIdentity, globalPackagesFolder);
            if (downloadResourceResult == null)
            {
                logger.Info($"Package {packageIdentity} not found in the global packages folder {globalPackagesFolder}");
                logger.Info($"Installing package {packageIdentity.Id}, Version {packageIdentity.Version}");
                var searchResult = nugetPackageSearcher.Search(packageIdentity);
                
                if (searchResult == null)
                {                    
                    return;
                }

                InstallPackage(searchResult);
            }
            

            downloadResourceResult = GlobalPackagesFolderUtility.GetPackage(packageIdentity, globalPackagesFolder);
            var supportedFrameworks = downloadResourceResult.PackageReader.GetSupportedFrameworks();
            var packageReader = downloadResourceResult.PackageReader;
            var versionFolderPathResolver = new VersionFolderPathResolver(globalPackagesFolder);           
            var installPath = versionFolderPathResolver.GetInstallPath(packageIdentity.Id, packageIdentity.Version);
            
            FrameworkReducer reducer = new FrameworkReducer();
            var nearest = reducer.GetNearest(framework, supportedFrameworks);

            if (nearest == null)
            {                                
                referencedPackages.Add(packageIdentity, Enumerable.Empty<string>());
                return;
            }
             
            var frameworkSpecificGroup = packageReader.GetLibItems().SingleOrDefault(i => i.TargetFramework == nearest);
            var files = frameworkSpecificGroup.Items.Select(i => i.ToLower()).Where(i => i.EndsWith("dll") && !i.EndsWith("resources.dll"));
            referencedPackages.Add(packageIdentity, files.Select(f => Path.GetFullPath(Path.Combine(installPath, f))));
        }

        private void InstallPackage(NuGetPackageSearchResult packageSearchResult)
        {
            //We simply install the package into a temporary folder 
            //Nuget will also install the package into the global packages folder.

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