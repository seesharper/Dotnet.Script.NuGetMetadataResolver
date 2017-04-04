namespace Dotnet.Script.NuGetMetadataResolver
{
    
    using System.Linq;    
    using Microsoft.Extensions.Logging;
    using NuGet.Configuration;
    

    /// <summary>
    /// A class that is capable of providing a list of possible package sources.
    /// </summary>
    public class NuGetPackageSourceProvider : INuGetPackageSourceProvider
    {
        private readonly string rootDirectory;
        private readonly ILogger logger;


        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackageSourceProvider"/> class.
        /// </summary>
       
        /// <param name="rootDirectory">The root directory from where to resolve possible package sources.</param>
        public NuGetPackageSourceProvider(string rootDirectory, ILoggerFactory loggerFactory)
        {
            this.rootDirectory = rootDirectory;
            this.logger = loggerFactory.CreateLogger<NuGetPackageSourceProvider>();
        }

        /// <inheritdoc />
        public PackageSource[] GetSources()
        {
            var defaultSettings = Settings.LoadDefaultSettings(rootDirectory);
            
            PackageSourceProvider nuGetPackageSourceProvider = new PackageSourceProvider(defaultSettings);
            var packageSources = nuGetPackageSourceProvider.LoadPackageSources().Where(ps => ps.IsEnabled).ToArray();                                               

            logger.LogInformation("Package sources;");
            foreach (var packageSource in packageSources)
            {
                logger.LogInformation($"{packageSource.Name} {packageSource.SourceUri}");
            }
            
            return packageSources.ToArray();
        }
    }
}