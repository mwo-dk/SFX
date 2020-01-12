using SFX.CSharp.ROP;
using SFX.CSharp.System.Windows.Model.Registry;
using System;
using static SFX.CSharp.System.Windows.Model.Registry.RegistryRootKeys;

namespace SFX.CSharp.System.Infrastructure
{
    /// <summary>
    /// Extension methods for <see cref="IRegistryReader"/>
    /// </summary>
    public static class IRegistryReaderExtensions
    {
        #region ClassesRoot
        /// <summary>
        /// Attempts to read the <see cref="RegistryStringValue"/> under ClassesRoot
        /// denoted by <paramref name="path"/>
        /// </summary>
        /// <param name="reader">The <see cref="IRegistryReader"/> utilized</param>
        /// <param name="path">The <see cref="RegistrySubPath"/> under <paramref name="rootKey"/> where the result has to be found</param>
        /// <param name="name">The <see cref="RegistryKeyName"/> of the key to be read</param>
        /// <returns>The result of the query</returns>
        public static Result<RegistryStringValue>
            ReadClassesRootStringValue(this IRegistryReader reader, RegistrySubPath path, RegistryKeyName name) =>
            reader.Safe().ReadStringValue(ClassesRoot, path, name);

        /// <summary>
        /// Attempts to read the <see cref="RegistryBlobValue"/> under ClassesRoot
        /// denoted by <paramref name="path"/>
        /// </summary>
        /// <param name="reader">The <see cref="IRegistryReader"/> utilized</param>
        /// <param name="path">The <see cref="RegistrySubPath"/> under <paramref name="rootKey"/> where the result has to be found</param>
        /// <param name="name">The <see cref="RegistryKeyName"/> of the key to be read</param>
        /// <returns>The result of the query</returns
        public static Result<RegistryBlobValue>
            ReadClassesRootBlobValue(this IRegistryReader reader, RegistrySubPath path, RegistryKeyName name) =>
            reader.Safe().ReadBlobValue(ClassesRoot, path, name);
        #endregion

        #region CurrentConfig
        /// <summary>
        /// Attempts to read the <see cref="RegistryStringValue"/> under CurrentConfig
        /// denoted by <paramref name="path"/>
        /// </summary>
        /// <param name="reader">The <see cref="IRegistryReader"/> utilized</param>
        /// <param name="path">The <see cref="RegistrySubPath"/> under <paramref name="rootKey"/> where the result has to be found</param>
        /// <param name="name">The <see cref="RegistryKeyName"/> of the key to be read</param>
        /// <returns>The result of the query</returns>
        public static Result<RegistryStringValue>
            ReadCurrentConfigStringValue(this IRegistryReader reader, RegistrySubPath path, RegistryKeyName name) =>
            reader.Safe().ReadStringValue(CurrentConfig, path, name);

        /// <summary>
        /// Attempts to read the <see cref="RegistryBlobValue"/> under CurrentConfig
        /// denoted by <paramref name="path"/>
        /// </summary>
        /// <param name="reader">The <see cref="IRegistryReader"/> utilized</param>
        /// <param name="path">The <see cref="RegistrySubPath"/> under <paramref name="rootKey"/> where the result has to be found</param>
        /// <param name="name">The <see cref="RegistryKeyName"/> of the key to be read</param>
        /// <returns>The result of the query</returns
        public static Result<RegistryBlobValue>
            ReadCurrentConfigBlobValue(this IRegistryReader reader, RegistrySubPath path, RegistryKeyName name) =>
            reader.Safe().ReadBlobValue(CurrentConfig, path, name);
        #endregion

        #region CurrentUser
        /// <summary>
        /// Attempts to read the <see cref="RegistryStringValue"/> under CurrentUser
        /// denoted by <paramref name="path"/>
        /// </summary>
        /// <param name="reader">The <see cref="IRegistryReader"/> utilized</param>
        /// <param name="path">The <see cref="RegistrySubPath"/> under <paramref name="rootKey"/> where the result has to be found</param>
        /// <param name="name">The <see cref="RegistryKeyName"/> of the key to be read</param>
        /// <returns>The result of the query</returns>
        public static Result<RegistryStringValue>
            ReadCurrentUserStringValue(this IRegistryReader reader, RegistrySubPath path, RegistryKeyName name) =>
            reader.Safe().ReadStringValue(CurrentUser, path, name);

        /// <summary>
        /// Attempts to read the <see cref="RegistryBlobValue"/> under CurrentUser
        /// denoted by <paramref name="path"/>
        /// </summary>
        /// <param name="reader">The <see cref="IRegistryReader"/> utilized</param>
        /// <param name="path">The <see cref="RegistrySubPath"/> under <paramref name="rootKey"/> where the result has to be found</param>
        /// <param name="name">The <see cref="RegistryKeyName"/> of the key to be read</param>
        /// <returns>The result of the query</returns
        public static Result<RegistryBlobValue>
            ReadCurrentUserBlobValue(this IRegistryReader reader, RegistrySubPath path, RegistryKeyName name) =>
            reader.Safe().ReadBlobValue(CurrentUser, path, name);
        #endregion

        #region LocalMachine
        /// <summary>
        /// Attempts to read the <see cref="RegistryStringValue"/> under LocalMachine
        /// denoted by <paramref name="path"/>
        /// </summary>
        /// <param name="reader">The <see cref="IRegistryReader"/> utilized</param>
        /// <param name="path">The <see cref="RegistrySubPath"/> under <paramref name="rootKey"/> where the result has to be found</param>
        /// <param name="name">The <see cref="RegistryKeyName"/> of the key to be read</param>
        /// <returns>The result of the query</returns>
        public static Result<RegistryStringValue>
            ReadLocalMachineStringValue(this IRegistryReader reader, RegistrySubPath path, RegistryKeyName name) =>
            reader.Safe().ReadStringValue(LocalMachine, path, name);

        /// <summary>
        /// Attempts to read the <see cref="RegistryBlobValue"/> under LocalMachine
        /// denoted by <paramref name="path"/>
        /// </summary>
        /// <param name="reader">The <see cref="IRegistryReader"/> utilized</param>
        /// <param name="path">The <see cref="RegistrySubPath"/> under <paramref name="rootKey"/> where the result has to be found</param>
        /// <param name="name">The <see cref="RegistryKeyName"/> of the key to be read</param>
        /// <returns>The result of the query</returns
        public static Result<RegistryBlobValue>
            ReadLocalMachineBlobValue(this IRegistryReader reader, RegistrySubPath path, RegistryKeyName name) =>
            reader.Safe().ReadBlobValue(LocalMachine, path, name);
        #endregion

        #region PerformanceData
        /// <summary>
        /// Attempts to read the <see cref="RegistryStringValue"/> under PerformanceData
        /// denoted by <paramref name="path"/>
        /// </summary>
        /// <param name="reader">The <see cref="IRegistryReader"/> utilized</param>
        /// <param name="path">The <see cref="RegistrySubPath"/> under <paramref name="rootKey"/> where the result has to be found</param>
        /// <param name="name">The <see cref="RegistryKeyName"/> of the key to be read</param>
        /// <returns>The result of the query</returns>
        public static Result<RegistryStringValue>
            ReadPerformanceDataStringValue(this IRegistryReader reader, RegistrySubPath path, RegistryKeyName name) =>
            reader.Safe().ReadStringValue(PerformanceData, path, name);

        /// <summary>
        /// Attempts to read the <see cref="RegistryBlobValue"/> under PerformanceData
        /// denoted by <paramref name="path"/>
        /// </summary>
        /// <param name="reader">The <see cref="IRegistryReader"/> utilized</param>
        /// <param name="path">The <see cref="RegistrySubPath"/> under <paramref name="rootKey"/> where the result has to be found</param>
        /// <param name="name">The <see cref="RegistryKeyName"/> of the key to be read</param>
        /// <returns>The result of the query</returns
        public static Result<RegistryBlobValue>
            ReadPerformanceDataBlobValue(this IRegistryReader reader, RegistrySubPath path, RegistryKeyName name) =>
            reader.Safe().ReadBlobValue(PerformanceData, path, name);
        #endregion

        #region Users
        /// <summary>
        /// Attempts to read the <see cref="RegistryStringValue"/> under Users
        /// denoted by <paramref name="path"/>
        /// </summary>
        /// <param name="reader">The <see cref="IRegistryReader"/> utilized</param>
        /// <param name="path">The <see cref="RegistrySubPath"/> under <paramref name="rootKey"/> where the result has to be found</param>
        /// <param name="name">The <see cref="RegistryKeyName"/> of the key to be read</param>
        /// <returns>The result of the query</returns>
        public static Result<RegistryStringValue>
            ReadUsersStringValue(this IRegistryReader reader, RegistrySubPath path, RegistryKeyName name) =>
            reader.Safe().ReadStringValue(Users, path, name);

        /// <summary>
        /// Attempts to read the <see cref="RegistryBlobValue"/> under Users
        /// denoted by <paramref name="path"/>
        /// </summary>
        /// <param name="reader">The <see cref="IRegistryReader"/> utilized</param>
        /// <param name="path">The <see cref="RegistrySubPath"/> under <paramref name="rootKey"/> where the result has to be found</param>
        /// <param name="name">The <see cref="RegistryKeyName"/> of the key to be read</param>
        /// <returns>The result of the query</returns
        public static Result<RegistryBlobValue>
            ReadUsersBlobValue(this IRegistryReader reader, RegistrySubPath path, RegistryKeyName name) =>
            reader.Safe().ReadBlobValue(Users, path, name);
        #endregion

        #region Helpers
        private static IRegistryReader Safe(this IRegistryReader reader) =>
            reader ?? throw new ArgumentNullException(nameof(reader));
        #endregion
    }
}
