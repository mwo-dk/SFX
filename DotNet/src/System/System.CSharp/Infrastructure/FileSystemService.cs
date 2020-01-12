using SFX.CSharp.ROP;
using SFX.CSharp.System.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static SFX.CSharp.ROP.Library;

namespace SFX.CSharp.System.Infrastructure
{
    /// <summary>
    /// Implements <see cref="IFileSystemService"/>
    /// </summary>
    public sealed class FileSystemService : IFileSystemService
    {
        /// <inheritdoc/>
        public Result<bool> FolderExists(FolderPath folderPath)
        {
            try
            {
                return Succeed(Directory.Exists(folderPath.Value));
            }
            catch (Exception exn)
            {
                return Fail<bool>(exn);
            }
        }


        /// <inheritdoc/>
        public Result<Unit> CreateFolder(FolderPath folderPath)
        {
            try
            {
                Directory.CreateDirectory(folderPath.Value);
                return Succeed(Unit.Value);
            }
            catch (Exception exn)
            {
                return Fail<Unit>(exn);
            }
        }

        /// <inheritdoc/>
        public Result<Unit> ClearFolder(FolderPath folderPath)
        {
            try
            {
                foreach (var folder in Directory.GetDirectories(folderPath.Value))
                    Directory.Delete(folder, true);
                foreach (var file in Directory.GetFiles(folderPath.Value))
                    File.Delete(file);
                return Succeed(Unit.Value);
            }
            catch (Exception exn)
            {
                return Fail<Unit>(exn);
            }
        }

        /// <inheritdoc/>
        public Result<bool> FileExists(FilePath filePath)
        {
            try
            {
                return Succeed(File.Exists(filePath.Value));
            }
            catch (Exception exn)
            {
                return Fail<bool>(exn);
            }
        }

        /// <inheritdoc/>
        public Result<IEnumerable<FilePath>> GetFiles(FolderPath folderPath)
        {
            try
            {
                var result = Directory.GetFiles(folderPath.Value)
                    .Select(filePath => new FilePath { Value = filePath });
                return Succeed(result);
            }
            catch (Exception exn)
            {
                return Fail<IEnumerable<FilePath>>(exn);
            }
        }

        /// <inheritdoc/>
        public Result<Unit> CreateFile(FilePath filePath, byte[] content)
        {
            if (content is null || content.Length == 0)
                return Fail<Unit>(new ArgumentNullException(nameof(content)));

            try
            {
                using var stream = File.Create(filePath.Value);
                stream.Write(content, 0, content.Length);
                return Succeed(Unit.Value);
            }
            catch (Exception exn)
            {
                return Fail<Unit>(exn);
            }
        }

        /// <inheritdoc/>
        public Result<Unit> CreateFile(FilePath filePath, string content)
        {
            if (string.IsNullOrEmpty(content))
                return Fail<Unit>(new ArgumentNullException(nameof(content)));

            try
            {
                using var stream = File.CreateText(filePath.Value);
                stream.Write(content);
                return Succeed(Unit.Value);
            }
            catch (Exception exn)
            {
                return Fail<Unit>(exn);
            }
        }

        /// <inheritdoc/>
        public async Task<Result<Unit>> CreateFileAsync(FilePath filePath, byte[] content)
        {
            if (content is null || content.Length == 0)
                return Fail<Unit>(new ArgumentNullException(nameof(content)));

            try
            {
                using var stream = File.Create(filePath.Value);
                await stream.WriteAsync(content, 0, content.Length);
                return Succeed(Unit.Value);
            }
            catch (Exception exn)
            {
                return Fail<Unit>(exn);
            }
        }

        /// <inheritdoc/>
        public async Task<Result<Unit>> CreateFileAsync(FilePath filePath, string content)
        {
            if (string.IsNullOrEmpty(content))
                return Fail<Unit>(new ArgumentNullException(nameof(content)));

            try
            {
                using var stream = File.CreateText(filePath.Value);
                await stream.WriteAsync(content);
                return Succeed(Unit.Value);
            }
            catch (Exception exn)
            {
                return Fail<Unit>(exn);
            }
        }

        /// <inheritdoc/>
        public Result<Unit> DeleteFile(FilePath filePath)
        {
            try
            {
                if (!File.Exists(filePath.Value))
                    return Fail<Unit>(new FileNotFoundException());
                File.Delete(filePath.Value);
                return Succeed(Unit.Value);
            }
            catch (Exception exn)
            {
                return Fail<Unit>(exn);
            }
        }

        /// <inheritdoc/>
        public Result<byte[]> ReadFileBinaryContent(FilePath filePath)
        {
            var (success, result, error) = FileExists(filePath);
            if (!success)
                return Fail<byte[]>(error);
            if (!result)
                return Fail<byte[]>(new FileNotFoundException());

            try
            {
                var info = new FileInfo(filePath.Value);
                var size = info.Length;
                var data = new byte[size];
                var count = 0L;
                var bufferSize = Int32.MaxValue < size ? (1 << 20) : ((int)size);
                byte[] buffer = new byte[bufferSize];

                using var file = File.OpenRead(filePath.Value);
                while (count < size)
                {
                    var n = file.Read(buffer, 0, bufferSize);
                    Array.Copy(buffer, 0, data, count, n);
                    count += n;
                }

                return Succeed(data);
            }
            catch (Exception exn)
            {
                return Fail<byte[]>(exn);
            }
        }

        /// <inheritdoc/>
        public Result<string> ReadFileStringContent(FilePath filePath)
        {
            var (success, result, error) = FileExists(filePath);
            if (!success)
                return Fail<string>(error);
            if (!result)
                return Fail<string>(new FileNotFoundException());

            try
            {
                using var file = File.OpenRead(filePath.Value);
                using var streamReader = new StreamReader(file);
                return Succeed(streamReader.ReadToEnd());
            }
            catch (Exception exn)
            {
                return Fail<string>(exn);
            }
        }

        /// <inheritdoc/>
        public async Task<Result<byte[]>> ReadFileBinaryContentAsync(FilePath filePath)
        {
            var (success, result, error) = FileExists(filePath);
            if (!success)
                return Fail<byte[]>(error);
            if (!result)
                return Fail<byte[]>(new FileNotFoundException());

            try
            {
                var info = new FileInfo(filePath.Value);
                var size = info.Length;
                var data = new byte[size];
                var count = 0L;
                var bufferSize = Int32.MaxValue < size ? (1 << 20) : ((int)size);
                byte[] buffer = new byte[bufferSize];

                using var file = File.OpenRead(filePath.Value);
                while (count < size)
                {
                    var n = await file.ReadAsync(buffer, 0, bufferSize);
                    Array.Copy(buffer, 0, data, count, n);
                    count += n;
                }

                return Succeed(data);
            }
            catch (Exception exn)
            {
                return Fail<byte[]>(exn);
            }
        }

        /// <inheritdoc/>
        public async Task<Result<string>> ReadFileStringContentAsync(FilePath filePath)
        {
            var (success, result, error) = FileExists(filePath);
            if (!success)
                return Fail<string>(error);
            if (!result)
                return Fail<string>(new FileNotFoundException());

            try
            {
                using var file = File.OpenRead(filePath.Value);
                using var streamReader = new StreamReader(file);
                return Succeed(await streamReader.ReadToEndAsync());
            }
            catch (Exception exn)
            {
                return Fail<string>(exn);
            }
        }
    }
}