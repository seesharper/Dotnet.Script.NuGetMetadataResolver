namespace Dotnet.Script.NuGetMetadataResolver
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a "project.json" files that can be saved to disc.
    /// </summary>
    public class ProjectJson
    {
        /// <summary>
        /// Gets a list of dependencies (references to NuGet packages)
        /// </summary>
        [JsonProperty("dependencies")]
        public Dictionary<string, string> Dependencies { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets a list of frameworks. This is used to specify the target framework when resolving runtime dependencies.
        /// </summary>
        [JsonProperty("frameworks")]
        public Dictionary<string, Dictionary<string,List<string>>> Frameworks { get; } = new Dictionary<string, Dictionary<string, List<string>>>();

        /// <summary>
        /// Save this project file to disc.
        /// </summary>
        /// <param name="path">The path to the project.json file. </param>
        public void Save(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {            
                using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    var content = JsonConvert.SerializeObject(this);
                    streamWriter.Write(content);
                }
            }
        }
    }   
}