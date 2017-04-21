namespace Dotnet.Script.NuGetMetadataResolver
{
    /// <summary>
    /// Represents a class that is capable of creating a 
    /// a project.json file based on the script files 
    /// found in a given directory.     
    /// </summary>
    public interface IScriptProjectProvider
    {
        /// <summary>
        /// Creates a project based on the script files found in the <paramref name="targetDirectory"/>.
        /// </summary>
        /// <param name="targetDirectory">The target directory containing one or more script files.</param>
        /// <param name="defaultTargetFramework">The target framework to be used unless we find a #! directive within the script files in the <paramref name="targetDirectory"/>.</param>
        /// <returns>The <see cref="ScriptProjectInfo"/> that represents the generated project.json file.</returns>
        ScriptProjectInfo CreateProject(string targetDirectory, string defaultTargetFramework = "net46");
    }
}