using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// Handles reading writing and querying the file system
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Writes a text to a specified file
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="path">File path</param>
        /// <param name="append">Append or not</param>
        /// <returns></returns>
        Task WriteTextToFileAsync(string text, string path, bool append = false);

        /// <summary>
        /// Normalizing a path based on the current operating system
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string NormalizePath(string path);

        /// <summary>
        /// Resolves any relative elements of a path to absolute
        /// </summary>
        /// <param name="path">Path to resolve</param>
        /// <returns></returns>
        string ResolvePath(string path);
    }
}
