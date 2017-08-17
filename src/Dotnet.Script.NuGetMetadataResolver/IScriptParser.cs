using Microsoft.CodeAnalysis.Text;

namespace Dotnet.Script.NuGetMetadataResolver
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a class that is capable of parsing a set of script files and scripts.
    /// and return information about NuGet references and the target framework.
    /// </summary>
    public interface IScriptParser
    {
        /// <summary>
        /// Parses the given set of <paramref name="csxFiles"/> and returns a <see cref="ParseResult"/>
        /// instance that contains a list of NuGet reference found within the script files.
        /// </summary>
        /// <param name="csxFiles">A list of script files for which to parse and resolve NuGet references.</param>
        /// <returns>A <see cref="ParseResult"/>
        /// instance that contains a list of NuGet reference found within the script files.</returns>
        ParseResult ParseFrom(IEnumerable<string> csxFiles);

        /// <summary>
        /// Parses the given set of <paramref name="csxSources"/> and returns a <see cref="ParseResult"/>
        /// instance that contains a list of NuGet reference found within the scripts.
        /// </summary>
        /// <param name="csxSources">A list of csx source for which to parse and resolve NuGet references.</param>
        /// <returns>A <see cref="ParseResult"/>
        /// instance that contains a list of NuGet reference found within the scripts.</returns>
        ParseResult ParseFrom(IEnumerable<SourceText> csxSources);
    }
}