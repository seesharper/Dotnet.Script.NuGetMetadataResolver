namespace Dotnet.Script.NuGetMetadataResolver
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
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
        private static string PathToNuget;

        static ScriptProjectProvider()
        {
            var directory = Path.GetDirectoryName(new Uri(typeof(ScriptProjectProvider).GetTypeInfo().Assembly.CodeBase).LocalPath);
            PathToNuget = Path.Combine(directory, "NuGet350.exe");
        }
        
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
            ExtractNugetExecutable();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                commandRunner.Execute(PathToNuget, $"restore {pathToProjectJson}");
            }
            else
            {
                commandRunner.Execute("mono", $"{PathToNuget} restore {pathToProjectJson}");
            }                        
        }

        private static string GetPathToProjectJson(string targetDirectory)
        {
            var tempDirectory = Path.GetTempPath();
            var pathRoot = Path.GetPathRoot(targetDirectory);
            var targetDirectoryWithoutRoot = targetDirectory.Substring(pathRoot.Length);
            if (pathRoot.Length > 0 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var driveLetter = pathRoot.Substring(0,1);
                targetDirectoryWithoutRoot = Path.Combine(driveLetter, targetDirectoryWithoutRoot);
            }
            var pathToProjectJsonDirectory = Path.Combine(tempDirectory, "scripts", targetDirectoryWithoutRoot);
            if (!Directory.Exists(pathToProjectJsonDirectory))
            {
                Directory.CreateDirectory(pathToProjectJsonDirectory);
            }
            var pathToProjectJson = Path.Combine(pathToProjectJsonDirectory, "project.json");
            return pathToProjectJson;
        }

        private void ExtractNugetExecutable()
        {
            if (!File.Exists(PathToNuget))
            {
                using (Stream input = typeof(ScriptProjectProvider).GetTypeInfo().Assembly.GetManifestResourceStream("Dotnet.Script.NuGetMetadataResolver.NuGet.NuGet.exe"))
                {

                    byte[] byteData = StreamToBytes(input);
                    File.WriteAllBytes(PathToNuget, byteData);
                }
            }                  
        }

        /// <summary>
        /// StreamToBytes - Converts a Stream to a byte array. Eg: Get a Stream from a file,url, or open file handle.
        /// </summary>
        /// <param name="input">input is the stream we are to return as a byte array</param>
        /// <returns>byte[] The Array of bytes that represents the contents of the stream</returns>
        static byte[] StreamToBytes(Stream input)
        {

            int capacity = input.CanSeek ? (int)input.Length : 0; //Bitwise operator - If can seek, Capacity becomes Length, else becomes 0.
            using (MemoryStream output = new MemoryStream(capacity)) //Using the MemoryStream output, with the given capacity.
            {
                int readLength;
                byte[] buffer = new byte[capacity/*4096*/];  //An array of bytes
                do
                {
                    readLength = input.Read(buffer, 0, buffer.Length);   //Read the memory data, into the buffer
                    output.Write(buffer, 0, readLength); //Write the buffer to the output MemoryStream incrementally.
                }
                while (readLength != 0); //Do all this while the readLength is not 0
                return output.ToArray();  //When finished, return the finished MemoryStream object as an array.
            }

        }
    }
}