namespace Dotnet.Script.NuGetMetadataResolver
{
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;

    public class NuGetPackageSearchResult
    {
        public PackageSource Source { get; set; }

        public IPackageSearchMetadata PackageSearchMetadata { get; set; }
    }
}