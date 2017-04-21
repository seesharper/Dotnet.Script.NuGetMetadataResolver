namespace Dotnet.Script.NuGetMetadataResolver
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A class that is capable of creating a 
    /// a project.json file based on the script files 
    /// found in a given directory.  
    /// </summary>
    public class ScriptProjectProvider : IScriptProjectProvider
    {
        private readonly ICommandRunner commandRunner;
        private readonly IScriptParser scriptParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptProjectProvider"/> class.
        /// </summary>
        /// <param name="commandRunner">The <see cref="ICommandRunner"/> that is responsible for executing the NuGet command.</param>
        /// <param name="scriptParser">The <see cref="IScriptParser"/> that is responsible for parsing NuGet references from script files.</param>
        public ScriptProjectProvider(ICommandRunner commandRunner, IScriptParser scriptParser)
        {
            this.commandRunner = commandRunner;
            this.scriptParser = scriptParser;
        }

        /// <summary>
        /// Creates a default <see cref="ScriptProjectProvider"/> instance.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> used to create an <see cref="ILogger"/> instance.</param>
        /// <returns></returns>
        public static ScriptProjectProvider Create(ILoggerFactory loggerFactory)
        {
            return new ScriptProjectProvider(new CommandRunner(loggerFactory), new ScriptParser(loggerFactory));
        }


        /// <inheritdoc />
        public ScriptProjectInfo CreateProject(string targetDirectory, string targetFramework = "net46")
        {
            var csxFiles = Directory.GetFiles(targetDirectory, "*.csx", SearchOption.AllDirectories);

            var parseresult = scriptParser.ParseFrom(csxFiles);
            if (parseresult.TargetFramework != null)
            {
                targetFramework = parseresult.TargetFramework;
            }

            var pathToProjectJson = GetPathToProjectJson(targetDirectory);
            var projectJson = new ProjectJson();
            projectJson.Frameworks.Add(targetFramework, new Dictionary<string, List<string>>());

            // Add the most common imports when resolving in the context of .Net Core
            if (targetFramework.StartsWith("netcoreapp", StringComparison.OrdinalIgnoreCase))
            {
                projectJson.Frameworks.First().Value.Add("imports", new List<string>(new[] { "dotnet", "dnxcore50" }));
            }
            
            var packageReferences = parseresult.PackageReferences;
            foreach (var packageReference in packageReferences)
            {
                projectJson.Dependencies.Add(packageReference.Id, packageReference.Version);
            }
            projectJson.Save(pathToProjectJson);
            Restore(pathToProjectJson);
            return new ScriptProjectInfo(pathToProjectJson, targetFramework);
        }

        private void Restore(string pathToProjectJson)
        {
            commandRunner.Execute("nuget", $"restore {pathToProjectJson}");
        }

        private static string GetPathToProjectJson(string targetDirectory)
        {
            var tempDirectory = Path.GetTempPath();
            
            var targetDirectoryWithoutRoot = targetDirectory.Substring(Path.GetPathRoot(targetDirectory).Length);
            var pathToProjectJsonDirectory = Path.Combine(tempDirectory, "scripts", targetDirectoryWithoutRoot);
            if (!Directory.Exists(pathToProjectJsonDirectory))
            {
                Directory.CreateDirectory(pathToProjectJsonDirectory);
            }
            var pathToProjectJson = Path.Combine(pathToProjectJsonDirectory, "project.json");
            return pathToProjectJson;
        }
    }
}