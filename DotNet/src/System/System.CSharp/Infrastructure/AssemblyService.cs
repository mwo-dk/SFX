using SFX.CSharp.ROP;
using SFX.CSharp.System.Model;
using System;
using System.IO;
using System.Reflection;
using static SFX.CSharp.ROP.Library;

namespace SFX.CSharp.System.Infrastructure
{
    /// <summary>
    /// Implementation of <see cref="IAssemblyService"/>
    /// </summary>
    public sealed class AssemblyService : IAssemblyService
    {
        /// <inheritdoc/>
        public Result<FilePath> GetExeFilePath()
        {
            try
            {
                var entry = Assembly.GetEntryAssembly();
                var codebase = entry.CodeBase;
                var path = new Uri(codebase).AbsolutePath;
                var cleanPath = Uri.UnescapeDataString(path);
                return Succeed(new FilePath { Value = Path.GetFullPath(cleanPath) });
            }
            catch (Exception error)
            {
                return Fail<FilePath>(error);
            }
        }
    }
}
