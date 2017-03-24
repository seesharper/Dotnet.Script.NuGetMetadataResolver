namespace Dotnet.Script.NuGetMetadataResolver
{
    using NuGet.Configuration;

    /// <summary>
    /// Represents a class that is capable of providing a list of possible package sources.
    /// </summary>
    public interface INuGetPackageSourceProvider
    {
        /// <summary>
        /// Gets a list of available package sources.
        /// </summary>
        /// <returns></returns>
        PackageSource[] GetSources();
    }
}