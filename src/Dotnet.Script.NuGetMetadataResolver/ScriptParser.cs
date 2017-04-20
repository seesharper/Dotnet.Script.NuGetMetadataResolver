namespace Dotnet.Script.NuGetMetadataResolver
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Logging;

    public class ScriptParser : IScriptParser
    {
        private readonly ILogger logger;
        public ScriptParser(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<ScriptParser>();
        }

        public ParseResult ParseFrom(IEnumerable<string> csxFiles)
        {
            HashSet<PackageReference> allPackageReferences = new HashSet<PackageReference>();                        
            string currentTargetFramework = null;
            foreach (var csxFile in csxFiles)
            {
                logger.LogDebug($"Parsing {csxFile}");
                var fileContent = ReadFile(csxFile);
                var packageReferences = ReadPackageReferences(fileContent);
                allPackageReferences.UnionWith(packageReferences);
                string targetFramework = ReadTargetFramework(fileContent);
                if (targetFramework != null)
                {
                    if (currentTargetFramework != null && targetFramework != currentTargetFramework)
                    {
                        logger.LogWarning($"Found multiple target frameworks. Using {currentTargetFramework}.");
                    }
                    else
                    {
                        currentTargetFramework = targetFramework;
                    }
                }
            }

            return new ParseResult(allPackageReferences, currentTargetFramework);
        }

        private IEnumerable<PackageReference> ReadPackageReferences(string fileContent)
        {
            const string pattern = @"#r\s*""nuget:\s*(.+)\s*,\s*(\d+\.\d+\.\d+)""";
            var matches = Regex.Matches(fileContent, pattern, RegexOptions.IgnoreCase);
            
            foreach (var match in matches.Cast<Match>())
            {
                var id = match.Groups[1].Value;
                var version = match.Groups[2].Value;
                var packageReference = new PackageReference(id, version);
                yield return packageReference;
            }
        }

        private string ReadTargetFramework(string fileContent)
        {
            const string pattern = @"#!\s*""(.*)""";
            var match = Regex.Match(fileContent, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return null;
        }

        private static string ReadFile(string pathToFile)
        {
            using (var fileStream = new FileStream(pathToFile,FileMode.Open))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    return reader.ReadToEnd();
                }
            }

            
        }

    }

    
}