namespace csx
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents a class that is capable of creating a 
    /// a project.json file based on the script files 
    /// found in a given directory.     
    /// </summary>
    public interface IScriptProjectProvider
    {
        /// <summary>
        /// Creates a project based on the script files found in the given <paramref name="csxFiles"/>.
        /// </summary>
        /// <param name="targetDirectory">The target directory containing one or more script files.</param>
        /// <param name="defaultTargetFramework">The target framework to be used unless we find a #! directive within the <paramref name="csxFiles"/>.</param>
        /// <returns>The path of the generated project file (project.json).</returns>
        string CreateProject(string targetDirectory, string defaultTargetFramework = "net46");
    }
}