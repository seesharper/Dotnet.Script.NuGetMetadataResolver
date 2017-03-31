namespace Dotnet.Script.NuGetMetadataResolver
{
    using System.Collections.Generic;
    using NuGet.Packaging.Core;

    /// <summary>
    /// Represents a class that is capable of installing a NuGet package 
    /// and return a list of files representing assemblies to be added as 
    /// metadata references.
    /// </summary>
    public interface INuGetPackageInstaller
    {
        /// <summary>
        /// Installs a NuGet package as identified by the <paramref name="packageIdentity"/>.
        /// </summary>
        /// <param name="packageIdentity">The <see cref="PackageIdentity"/> of the NuGet package to install.</param>
        /// <returns>A list of files representing assemblies to be added as metadata references.</returns>
        void Install(Dictionary<PackageIdentity, IEnumerable<string>> referencedPackages,PackageIdentity packageIdentity);
    }
}