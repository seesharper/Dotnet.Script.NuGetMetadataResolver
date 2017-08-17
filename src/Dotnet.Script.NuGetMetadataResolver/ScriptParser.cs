﻿using Microsoft.CodeAnalysis.Text;

namespace Dotnet.Script.NuGetMetadataResolver
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A class that is capable of parsing a set of script files 
    /// and return information about NuGet references and the target framework.
    /// </summary>
    public class ScriptParser : IScriptParser
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new insstance of the <see cref="ScriptParser"/> class.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> used to create an <see cref="ILogger"/> instance.</param>
        public ScriptParser(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<ScriptParser>();
        }

        /// <inheritdoc />
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

        public ParseResult ParseFrom(IEnumerable<SourceText> csxSources)
        {
            HashSet<PackageReference> allPackageReferences = new HashSet<PackageReference>();                        
            string currentTargetFramework = null;
            var count = 0;
            foreach (var csxSource in csxSources)
            {
                var sourceTextString = csxSource.ToString();
                logger.LogDebug($"Parsing source index {count++}");
                var packageReferences = ReadPackageReferences(sourceTextString);
                allPackageReferences.UnionWith(packageReferences);
                string targetFramework = ReadTargetFramework(sourceTextString);
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
            const string pattern = @"^\s*#r\s*""nuget:\s*(.+)\s*,\s*(.*)""";
            var matches = Regex.Matches(fileContent, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            
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
            const string pattern = @"^\s*#!\s*""(.*)""";
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