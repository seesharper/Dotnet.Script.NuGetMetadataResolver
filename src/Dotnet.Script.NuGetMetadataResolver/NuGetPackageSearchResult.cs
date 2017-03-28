namespace Dotnet.Script.NuGetMetadataResolver
{
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;

    /// <summary>
    /// Represents the result of searching for a NuGet package.
    /// </summary>
    public class NuGetPackageSearchResult
    {
        /// <summary>
        /// Gets or sets the <see cref="PackageSource"/> where the package was found.
        /// </summary>
        public PackageSource Source { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPackageSearchMetadata"/> that describes the package.
        /// </summary>
        public IPackageSearchMetadata PackageSearchMetadata { get; set; }
    }
}