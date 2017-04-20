namespace Dotnet.Script.NuGetMetadataResolver
{
    using System.Collections.Generic;

    public interface IScriptParser
    {
        ParseResult ParseFrom(IEnumerable<string> csxFiles);
    }
}