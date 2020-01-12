using SFX.CSharp.ROP;
using SFX.CSharp.System.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFX.CSharp.System.Infrastructure
{
    /// <summary>
    /// Interface describing access to the local file system
    /// </summary>
    public interface IFileSystemService
    {
        /// <summary>
        /// Determines whether a given folder exists.
        /// </summary>
        /// <param name="folderPath">The <see cref="FolderPath"/> to the given folder</param>
        /// <returns>True if the folder exists. Else false</returns>
        Result<bool> FolderExists(FolderPath folderPath);

        /// <summary>
        /// Creates a folder denoted by the given <paramref name="folderPath"/>
        /// </summary>
        /// <param name="folderPath">The <see cref="FolderPath"/> to the folder to be created</param>
        Result<Unit> CreateFolder(FolderPath folderPath);

        /// <summary>
        /// Clears folder denoted by the given <paramref name="folderPath"/>
        /// </summary>
        /// <param name="folderPath">The <see cref="FolderPath"/> to the folder to be cleared</param>
        Result<Unit> ClearFolder(FolderPath folderPath);

        /// <summary>
        /// Determines whether a given file exists.
        /// </summary>
        /// <param name="filePath">The <see cref="FilePath"/> to the given file</param>
        /// <returns>True if the file exists. Else false</returns>
        Result<bool> FileExists(FilePath filePath);

        /// <summary>
        /// Gets a list of files in the given <paramref name="folderPath"/>
        /// </summary>
        /// <param name="folderPath">The <see cref="FolderPath"/> to the given folder</param>
        /// <returns>A sequence of <see cref="FilePath"/>s to the files in <paramref name="folderPath"/></returns>
        Result<IEnumerable<FilePath>> GetFiles(FolderPath folderPath);

        /// <summary>
        /// Creates a file denoted by the given <paramref name="filePath"/> and writes the content <paramref name="content"/> to it
        /// </summary>
        /// <param name="filePath">The <see cref="FilePath"/> to the file to be created</param>
        /// <param name="content">The content to be written to the new file</param>
        Result<Unit> CreateFile(FilePath filePath, byte[] content);

        /// <summary>
        /// Creates a file denoted by the given <paramref name="filePath"/> and writes the content <paramref name="content"/> to it
        /// </summary>
        /// <param name="filePath">The <see cref="FilePath"/> to the file to be created</param>
        /// <param name="content">The content to be written to the new file</param>
        Result<Unit> CreateFile(FilePath filePath, string content);

        /// <summary>
        /// Creates a file denoted by the given <paramref name="filePath"/> and writes the content <paramref name="content"/> to it
        /// </summary>
        /// <param name="filePath">The <see cref="FilePath"/> to the file to be created</param>
        /// <param name="content">The content to be written to the new file</param>
        Task<Result<Unit>> CreateFileAsync(FilePath filePath, byte[] content);

        /// <summary>
        /// Creates a file denoted by the given <paramref name="filePath"/> and writes the content <paramref name="content"/> to it
        /// </summary>
        /// <param name="filePath">The <see cref="FilePath"/> to the file to be created</param>
        /// <param name="content">The content to be written to the new file</param>
        Task<Result<Unit>> CreateFileAsync(FilePath filePath, string content);

        /// <summary>
        ///  Creates a file denoted by the given <paramref name="filePath"/> 
        /// </summary>
        /// <param name="filePath">The <see cref="FilePath"/> to the file to be deleted</param>
        Result<Unit> DeleteFile(FilePath filePath);

        /// <summary>
        /// Reads the content of the file denoted by <paramref name="filePath"/>
        /// </summary>
        /// <param name="filePath">The <see cref="FilePath"/> of the file whose binary content is to be read</param>
        /// <returns>The binary content of the file denoted by <paramref name="filePath"/></returns>
        Result<byte[]> ReadFileBinaryContent(FilePath filePath);

        /// <summary>
        /// Reads the content of the file denoted by <paramref name="filePath"/>
        /// </summary>
        /// <param name="filePath">The <see cref="FilePath"/> of the file whose <see cref="string"/> content is to be read</param>
        /// <returns>The <see cref="string"/> content of the file denoted by <paramref name="filePath"/></returns>
        Result<string> ReadFileStringContent(FilePath filePath);

        /// <summary>
        /// Reads the content of the file denoted by <paramref name="filePath"/>
        /// </summary>
        /// <param name="filePath">The <see cref="FilePath"/> of the file whose binary content is to be read</param>
        /// <returns>The binary content of the file denoted by <paramref name="filePath"/></returns>
        Task<Result<byte[]>> ReadFileBinaryContentAsync(FilePath filePath);

        /// <summary>
        /// Reads the content of the file denoted by <paramref name="filePath"/>
        /// </summary>
        /// <param name="filePath">The <see cref="FilePath"/> of the file whose <see cref="string"/> content is to be read</param>
        /// <returns>The <see cref="string"/> content of the file denoted by <paramref name="filePath"/></returns>
        Task<Result<string>> ReadFileStringContentAsync(FilePath filePath);
    }
}
