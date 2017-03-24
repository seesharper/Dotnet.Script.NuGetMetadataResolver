namespace Dotnet.Script.NuGetMetadataResolver
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Extensions.Logging;
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
        private readonly ILoggerFactory loggerFactory;

        public NuGetPackageSearcher(INuGetPackageSourceProvider nuGetPackageSourceProvider, ILoggerFactory loggerFactory)
        {
            this.nuGetPackageSourceProvider = nuGetPackageSourceProvider;
            this.loggerFactory = loggerFactory;
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

        private NuGetPackageSearchResult Search(PackageSource packageSource, PackageIdentity packageIdentity)
        {
            var logger = new NuGetLogger(loggerFactory);            
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
            private readonly ILogger logger;

            public NuGetLogger(ILoggerFactory loggerFactory)
            {
                logger = loggerFactory.CreateLogger<NuGetLogger>();
            }

            public void LogDebug(string data)
            {
                logger.LogDebug(data);
            }

            public void LogError(string data)
            {
                logger.LogError(data);
            }

            public void LogErrorSummary(string data)
            {
                logger.LogError(data);
            }

            public void LogInformation(string data)
            {
                logger.LogInformation(data);
            }

            public void LogInformationSummary(string data)
            {
                logger.LogInformation(data);
            }

            public void LogMinimal(string data)
            {
                logger.LogInformation(data);
            }

            public void LogVerbose(string data)
            {
                logger.LogInformation(data);
            }

            public void LogWarning(string data)
            {
                logger.LogWarning(data);
            }
        }
    }
}