namespace Dotnet.Script.NuGetMetadataResolver
{
    using System;
    using System.IO;

    public class FolderUtils
    {
        /// <summary>
        /// Tries really hard to delete a folder.
        /// </summary>
        /// <param name="path">The path to the folder to be deleted.</param>
        public static void Delete(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            // http://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true
            foreach (string directory in Directory.GetDirectories(path))
            {
                Delete(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }
    }
}