namespace Dotnet.Script.NuGetMetadataResolver
{
    using System;
    using System.Linq;
    using Logging;
    using NuGet.Configuration;
    

    /// <summary>
    /// A class that is capable of providing a list of possible package sources.
    /// </summary>
    public class NuGetPackageSourceProvider : INuGetPackageSourceProvider
    {
        private readonly string rootDirectory;
        private readonly Action<LogEntry> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackageSourceProvider"/> class.
        /// </summary>
       
        /// <param name="rootDirectory">The root directory from where to resolve possible package sources.</param>
        public NuGetPackageSourceProvider(string rootDirectory)
        {
            this.rootDirectory = rootDirectory;
            logger = LogFactory.GetLogger<NuGetPackageSourceProvider>();
        }

        /// <inheritdoc />
        public PackageSource[] GetSources()
        {
            var defaultSettings = Settings.LoadDefaultSettings(rootDirectory);
            
            PackageSourceProvider nuGetPackageSourceProvider = new PackageSourceProvider(defaultSettings);
            var packageSources = nuGetPackageSourceProvider.LoadPackageSources().Where(ps => ps.IsEnabled).ToArray();                                               

            logger.Info("Package sources;");
            foreach (var packageSource in packageSources)
            {
                logger.Info($"{packageSource.Name} {packageSource.SourceUri}");
            }
            
            return packageSources.ToArray();
        }
    }
}