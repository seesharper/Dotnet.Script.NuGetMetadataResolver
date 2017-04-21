namespace Dotnet.Script.NuGetMetadataResolver
{
    /// <summary>
    /// Represents the result of creating a project 
    /// from a set of script files.
    /// </summary>
    public class ScriptProjectInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptProjectInfo"/> class.
        /// </summary>
        /// <param name="pathToProjectJson">The path to the generated project.json file.</param>
        /// <param name="targetFramework">The target framework.</param>
        public ScriptProjectInfo(string pathToProjectJson, string targetFramework)
        {
            PathToProjectJson = pathToProjectJson;
            TargetFramework = targetFramework;
        }
        
        /// <summary>
        /// Gets the path to the generated project.json file.
        /// </summary>
        public string PathToProjectJson { get; }

        /// <summary>
        /// Gets the target framework.
        /// </summary>
        public string TargetFramework { get; }
    }
}