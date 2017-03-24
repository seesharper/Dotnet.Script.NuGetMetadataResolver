namespace Dotnet.Script.NuGetMetadataResolver
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.Logging;
    using NuGet.Packaging.Core;
    using NuGet.Versioning;

    /// <summary>
    /// A <see cref="MetadataReferenceResolver"/> decorator that is capable of resolving 
    /// #r directives that references NuGet packages (#r nuget:PackageId/Version)
    /// </summary>
    public class NuGetMetadataReferenceResolver : MetadataReferenceResolver
    {
        private readonly MetadataReferenceResolver metadataReferenceResolver;        
        private readonly INuGetPackageInstaller nuGetPackageInstaller;        
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetMetadataReferenceResolver"/> class.
        /// </summary>
        /// <param name="metadataReferenceResolver">The target <see cref="MetadataReferenceResolver"/>.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> that is responsible for creating <see cref="ILogger"/> instances.</param>
        /// <param name="nuGetPackageInstaller">The <see cref="INuGetPackageInstaller"/> that is responsible for installing NuGet packages.</param>
        public NuGetMetadataReferenceResolver(MetadataReferenceResolver metadataReferenceResolver, ILoggerFactory loggerFactory, INuGetPackageInstaller nuGetPackageInstaller)
        {
            logger = loggerFactory.CreateLogger<NuGetMetadataReferenceResolver>();
            this.metadataReferenceResolver = metadataReferenceResolver;            
            this.nuGetPackageInstaller = nuGetPackageInstaller;            
        }

        /// <inheritdoc />
        public override bool Equals(object other)
        {
            return metadataReferenceResolver.Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return metadataReferenceResolver.GetHashCode();                     
        }

        public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string baseFilePath, MetadataReferenceProperties properties)
        {
            if (reference.StartsWith("nuget", StringComparison.OrdinalIgnoreCase))
            {
                var packageIdentity = ParseNugetReference(reference);
                if (packageIdentity != null)
                {
                    logger.LogInformation($"Found Nuget reference {reference}");
                    var metadataReferenceFiles = nuGetPackageInstaller.Install(packageIdentity);                    
                    return metadataReferenceFiles.Select(mrf => MetadataReference.CreateFromFile(mrf)).ToImmutableArray();                    
                }                
            }

            return metadataReferenceResolver.ResolveReference(reference, baseFilePath, properties);
        }

        private static PackageIdentity ParseNugetReference(string nuGetReference)
        {
            // Require Major, Minor and Revision before considering the reference to be valid.
            // This is to prevent premature installalation of packages during typing in the editor.

            var regex = new Regex(@"(.+)\/(\d\.\d\.\d)");
            var match = regex.Match(nuGetReference);
            if (match.Success)
            {
                var packageName = match.Groups[1].Value;
                var version = match.Groups[2].Value;
                NuGetVersion nuGetVersion;
                NuGetVersion.TryParseStrict(version, out nuGetVersion);
                if (nuGetVersion != null)
                {
                    return new PackageIdentity(packageName, nuGetVersion);
                }
            }
            return null;
        }
    }
}
