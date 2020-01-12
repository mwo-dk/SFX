using AutoFixture;
using SFX.CSharp.System.Infrastructure;
using SFX.CSharp.System.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace SFX.System.Test.SystemIntegration.Infrastructure
{
    [Trait("Category", "SystemIntegration")]
    public class FileSystemServiceTests
    {
        #region Members
        private readonly Fixture _fixture;
        private List<string> _folders = new List<string>();
        private List<string> _files = new List<string>();
        private readonly ITestOutputHelper _output;
        private static readonly string WorkingFolder = Path.Combine(Directory.GetCurrentDirectory(), "WorkingFolder");
        #endregion

        #region Test initialization
        public FileSystemServiceTests(ITestOutputHelper output)
        {
            _fixture = new Fixture().Customize(new SupportMutableValueTypesCustomization()) as Fixture;
            _output = output;
            CreateWorkingFolder();
        }
        #endregion

        #region Folder existence tests
        [Fact]
        public void If_test_folder_is_not_created_FolderExists_returns_false()
        {
            var sut = new FileSystemService();

            var (success, result, error) = sut.FolderExists(GetRandomFolder());

            Assert.True(success);
            Assert.Null(error);
            Assert.False(result);
        }

        [Fact]
        public void If_test_folder_is_created_FolderExists_returns_false()
        {
            var sut = new FileSystemService();
            var folder = GetRandomFolder();
            CreateFolder(folder.Value);

            var (success, result, error) = sut.FolderExists(folder);

            Assert.True(success);
            Assert.Null(error);
            Assert.True(result);
        }
        #endregion

        #region Create folder tests
        [Fact(Skip = "Works different on Ubuntu")]
        public void CreateFolder_creates_new_empty_folder()
        {
            var folder = GetRandomFolder();
            var sut = new FileSystemService();

            sut.CreateFolder(folder);
            _output.WriteLine("Folder: {0}", folder.Value);
            _output.WriteLine("Folders: {0}", string.Join(',', GetFolders()));

            Assert.Contains(GetFolders(), folder_ => new FolderPath { Value = folder_ }.Equals(folder));
        }
        #endregion

        #region Clear folder tests
        [Fact]
        public void ClearFolder_removes_existing_folders()
        {
            var folders = Enumerable.Range(0, 10)
                .Select(_ => GetRandomFolder())
                .ToArray();
            var sut = new FileSystemService();
            foreach (var folder in folders)
                CreateFolder(folder.Value);

            sut.ClearFolder(new FolderPath { Value = WorkingFolder });

            Assert.True(!GetFolders().Any());
        }

        [Fact]
        public void ClearFolder_removes_existing_files()
        {
            var files = Enumerable.Range(0, 10)
                .Select(_ => GetRandomFile())
                .ToArray();
            var sut = new FileSystemService();
            foreach (var filePath in files)
                CreateFile(filePath.Value, _fixture.Create<string>());

            sut.ClearFolder(new FolderPath { Value = WorkingFolder });

            Assert.True(!GetFolders().Any());
        }
        #endregion

        #region File existence tests
        [Fact]
        public void If_file_is_not_created_FileExists_return_false()
        {
            var sut = new FileSystemService();

            var (success, result, error) = sut.FileExists(new FilePath { Value = Path.Combine(WorkingFolder, _fixture.Create<string>()) });

            Assert.True(success);
            Assert.Null(error);
            Assert.False(result);
        }

        [Fact]
        public void If_file_is_created_FileExists_return_true()
        {
            var fileName = Path.Combine(WorkingFolder, _fixture.Create<string>());
            var sut = new FileSystemService();
            CreateFile(fileName, _fixture.Create<string>());

            var (success, result, error) = sut.FileExists(new FilePath { Value = fileName });

            Assert.True(success);
            Assert.Null(error);
            Assert.True(result);
        }
        #endregion

        #region File listing tests
        [Fact]
        public void GetFiles_returns_exact_list_of_files_in_folder()
        {
            var fileNames = _fixture.CreateMany<string>().ToArray();
            var filePaths =
                fileNames.Select(fileName => new FilePath { Value = Path.Combine(WorkingFolder, fileName) })
                    .ToArray();
            var sut = new FileSystemService();
            foreach (var filePath in filePaths)
                CreateFile(filePath.Value, _fixture.Create<string>());

            var (success, files_, error) = sut.GetFiles(new FolderPath { Value = WorkingFolder });

            Assert.True(success);
            Assert.Null(error);
            Assert.NotNull(files_);

            var files = files_.ToArray();

            Assert.Equal(fileNames.Length, files.Length);
            foreach (var file in files)
                Assert.Contains(filePaths, filePath => file.Equals(filePath));
        }
        #endregion

        #region Create file tests
        [Fact]
        public void CreateFile_creates_a_file_with_expected_content()
        {
            var fileName = Path.Combine(WorkingFolder, _fixture.Create<string>());
            var fileContent = _fixture.Create<string>();
            var sut = new FileSystemService();

            sut.CreateFile(new FilePath { Value = fileName }, fileContent);

            var content = ReadFile(fileName);

            Assert.Equal(fileContent, content);
        }
        #endregion

        #region Delete file tests
        [Fact]
        public void DeleteFile_works()
        {
            var fileName = Path.Combine(WorkingFolder, _fixture.Create<string>());
            var fileContent = _fixture.Create<string>();
            var sut = new FileSystemService();

            sut.CreateFile(new FilePath { Value = fileName }, fileContent);
            sut.DeleteFile(new FilePath { Value = fileName });

            var (ok, result, error) = sut.FileExists(new FilePath { Value = fileName });
            Assert.True(ok);
            Assert.Null(error);
            Assert.False(result);
        }
        #endregion

        #region Read file tests
        [Fact]
        public void ReadFile_returns_expected_content()
        {
            var fileName = Path.Combine(WorkingFolder, _fixture.Create<string>());
            var fileContent = _fixture.Create<string>();
            var sut = new FileSystemService();
            CreateFile(fileName, fileContent);

            sut.CreateFile(new FilePath { Value = fileName }, fileContent);

            var (success, content, error) = sut.ReadFileStringContent(new FilePath { Value = fileName });

            Assert.True(success);
            Assert.Null(error);
            Assert.Equal(fileContent, content);
        }
        #endregion

        #region Utility
        private static string GetRandomName() => Guid.NewGuid().ToString("N").Substring(0, 8);
        private FolderPath GetRandomFolder()
        {
            var name = GetRandomName();
            var path = Path.Combine(Directory.GetCurrentDirectory(), $@"WorkingFolder\{name}");
            _folders.Add(path);
            return new FolderPath { Value = path };
        }
        private FilePath GetRandomFile()
        {
            var name = GetRandomName();
            var path = Path.Combine(Directory.GetCurrentDirectory(), $@"WorkingFolder\{name}");
            _files.Add(path);
            return new FilePath { Value = path };
        }

        private static void CreateWorkingFolder()
        {
            CreateFolder(WorkingFolder);
        }
        private void RemoveFilesAndFolders()
        {
            foreach (var folder in _folders)
                RemoveFolder(folder);
            foreach (var file in _files)
                RemoveFile(file);
            RemoveFolder(WorkingFolder);
        }

        private static void DoSafe(Action action)
        {
            try
            {
                action();
            }
            catch { }
        }
        private static string[] GetFolders() =>
            Directory.GetDirectories(WorkingFolder);
        private static void CreateFolder(string path) =>
            DoSafe(() => Directory.CreateDirectory(path));
        private static void RemoveFolder(string path) =>
            DoSafe(() => Directory.Delete(path));
        private static string[] GetFiles() => Directory.GetFiles(WorkingFolder);

        private static string ReadFile(string path)
        {
            using (var file = File.OpenRead(path))
            {
                using (var streamReader = new StreamReader(file))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
        private static StreamWriter CreateFile(string path) => File.CreateText(path);

        private static void CreateFile(string path, string content)
        {
            using (var streamWriter = CreateFile(path))
            {
                streamWriter.Write(content);
            }
        }
        private static void RemoveFile(string path) => File.Delete(path);

        #endregion

        public void Dispose()
        {
            RemoveFilesAndFolders();
        }
    }
}
