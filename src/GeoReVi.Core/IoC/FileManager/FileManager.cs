using System.IO;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// Handles reading writing and querying the file system
    /// </summary>
    public class FileManager : IFileManager
    {
        /// <summary>
        /// Normalizing a path based on the current operating system
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string NormalizePath(string path)
        {
            switch(System.Environment.OSVersion.Platform)
            {
                case System.PlatformID.Win32NT:
                    return path?.Replace('/', '\\');
                case System.PlatformID.Win32S:
                    return path?.Replace('/', '\\');
                case System.PlatformID.Win32Windows:
                    return path?.Replace('/', '\\');
                case System.PlatformID.WinCE:
                    return path?.Replace('/', '\\');
                default:
                    return path?.Replace('\\', '/').Trim();

            }
        }

        /// <summary>
        /// Resolves any relative elements of a path to absolute
        /// </summary>
        /// <param name="path">Path to resolve</param>
        /// <returns></returns>
        public string ResolvePath(string path)
        {
            //Returning the complete path of a relative path
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Writes a text to a specified file
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="path">File path</param>
        /// <param name="append">Append or not</param>
        /// <returns></returns>
        public async Task WriteTextToFileAsync(string text, string path, bool append = false)
        {
            //TODO Add exception catching

            //Normalizing the path
            path = NormalizePath(path);

            path = ResolvePath(path);

            //Lock the Task
            await AsyncAwaiter.AwaitAsync(nameof(WriteTextToFileAsync) + path, async () =>
            {
                //TODO Add IoC.Task.Run that logs to logger on failure

                //Run the synchronous file access as a new task
                await Task.Run(()=>
                {
                    // Writing to the file
                    using (var fileStream = (TextWriter)new StreamWriter(File.Open(path, append ? FileMode.Append : FileMode.Create)))
                        fileStream.Write(text);
                });
            });

        }
    }
}
