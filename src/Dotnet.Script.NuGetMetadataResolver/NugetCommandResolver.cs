using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Dotnet.Script.NuGetMetadataResolver
{
    public class NugetCommandResolver : INugetCommandResolver
    {
        private static readonly string PathToNuget;

        static NugetCommandResolver()
        {
            var directory = Path.GetDirectoryName(new Uri(typeof(ScriptProjectProvider).GetTypeInfo().Assembly.CodeBase)
                .LocalPath);
            PathToNuget = Path.Combine(directory, "NuGet350.exe");
        }


        public string ResolveNugetCommand()
        {
            ExtractNugetExecutable(PathToNuget);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return PathToNuget;
            }
            return MonoCommand(PathToNuget);
        }

        protected virtual string MonoCommand(string pathToNuget)
        {
            return "mono " + PathToNuget;
        }

        private void ExtractNugetExecutable(string pathToNuget)
        {
            if (!File.Exists(pathToNuget))
            {
                using (Stream input = typeof(ScriptProjectProvider).GetTypeInfo().Assembly.GetManifestResourceStream("Dotnet.Script.NuGetMetadataResolver.NuGet.NuGet.exe"))
                using (Stream output = File.OpenWrite(pathToNuget))
                {
                    CopyStream(input, output);
                }
            }
        }

        /// <summary>
        /// Copies the contents of input to output. Doesn't close either stream.
        /// </summary>
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }
    }
}
