namespace Dotnet.Script.NuGetMetadataResolver
{
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;

    /// <summary>
    /// Represents a class that is capable of searching for a given <see cref="PackageIdentity"/>.
    /// </summary>
    public interface INugetPackageSearcher
    {
        /// <summary>
        /// Searches for a NuGet package identified by the given <paramref name="packageIdentity"/>.
        /// </summary>
        /// <param name="packageIdentity">The <see cref="PackageIdentity"/> of the NuGet package to search for.</param>
        /// <returns>An <see cref="NuGetPackageSearchResult"/> instance if the package is found, otherwise null.</returns>
        NuGetPackageSearchResult Search(PackageIdentity packageIdentity);
    }
}