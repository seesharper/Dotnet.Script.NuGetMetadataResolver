namespace csx
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Dotnet.Script.NuGetMetadataResolver;
    using Microsoft.Extensions.Logging;

    public class ScriptProjectProvider : IScriptProjectProvider
    {
        private readonly ICommandRunner commandRunner;
        private readonly IScriptParser scriptParser;

        public ScriptProjectProvider(ICommandRunner commandRunner, IScriptParser scriptParser)
        {
            this.commandRunner = commandRunner;
            this.scriptParser = scriptParser;
        }

        public static ScriptProjectProvider Create(ILoggerFactory loggerFactory)
        {
            return new ScriptProjectProvider(new CommandRunner(loggerFactory), new ScriptParser(loggerFactory));
        }


        public string CreateProject(string targetDirectory, string targetFramework = "net46")
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
            return pathToProjectJson;
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