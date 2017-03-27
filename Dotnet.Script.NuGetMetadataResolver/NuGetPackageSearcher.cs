namespace Dotnet.Script.NuGetMetadataResolver
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Logging;    
    using NuGet.Configuration;
    using NuGet.Packaging.Core;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;

    /// <summary>
    /// A <see cref="INugetPackageSearcher"/> that searches multiple packge sources 
    /// for a given <see cref="PackageIdentity"/>.
    /// </summary>
    public class NuGetPackageSearcher : INugetPackageSearcher
    {
        private readonly INuGetPackageSourceProvider nuGetPackageSourceProvider;


        public NuGetPackageSearcher(INuGetPackageSourceProvider nuGetPackageSourceProvider)
        {
            this.nuGetPackageSourceProvider = nuGetPackageSourceProvider;            
        }

        /// <inheritdoc />
        public NuGetPackageSearchResult Search(PackageIdentity packageIdentity)
        {
            
            var sources = nuGetPackageSourceProvider.GetSources();
            foreach (var source in sources)
            {
                var result = Search(source, packageIdentity);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private static NuGetPackageSearchResult Search(PackageSource packageSource, PackageIdentity packageIdentity)
        {
            var logger = new NuGetLogger();            
            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());            
            SourceRepository sourceRepository = new SourceRepository(packageSource, providers);
            PackageMetadataResource packageMetadataResource = sourceRepository.GetResource<PackageMetadataResource>();
            var result =
                packageMetadataResource.GetMetadataAsync(packageIdentity, logger, CancellationToken.None)
                    .Result;            
            if (result != null)
            {
                return new NuGetPackageSearchResult()
                {
                    PackageSearchMetadata = result,
                    Source = packageSource
                };
            }
            return null;
        }

        private class NuGetLogger : NuGet.Common.ILogger
        {
            private readonly Action<LogEntry> logger;

            public NuGetLogger()
            {
                logger = LogFactory.GetLogger<NuGetLogger>();
            }

            public void LogDebug(string data)
            {
                logger.Debug(data);
            }

            public void LogError(string data)
            {
                logger.Error(data);
            }

            public void LogErrorSummary(string data)
            {
                logger.Error(data);
            }

            public void LogInformation(string data)
            {
                logger.Info(data);
            }

            public void LogInformationSummary(string data)
            {
                logger.Info(data);
            }

            public void LogMinimal(string data)
            {
                logger.Info(data);
            }

            public void LogVerbose(string data)
            {
                logger.Info(data);
            }

            public void LogWarning(string data)
            {
                logger.Warning(data);
            }
        }
    }
}