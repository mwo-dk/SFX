namespace SFX.CSharp.System.Windows.Model.Registry
{
    /// <summary>
    /// Placeholder for relevant <see cref="RegistryRootKey"/>s
    /// </summary>
    public static class RegistryRootKeys
    {
        /// <summary>
        /// Root key for classes root
        /// </summary>
        public static RegistryRootKey ClassesRoot =>
            new RegistryRootKey(Microsoft.Win32.Registry.ClassesRoot);
        /// <summary>
        /// Root key for current config
        /// </summary>
        public static RegistryRootKey CurrentConfig =>
            new RegistryRootKey(Microsoft.Win32.Registry.CurrentConfig);
        /// <summary>
        /// Root key for current user
        /// </summary>
        public static RegistryRootKey CurrentUser =>
            new RegistryRootKey(Microsoft.Win32.Registry.CurrentUser);
        /// <summary>
        /// Root key for local machine
        /// </summary>
        public static RegistryRootKey LocalMachine =>
            new RegistryRootKey(Microsoft.Win32.Registry.LocalMachine);
        /// <summary>
        /// Root key for performance data
        /// </summary>
        public static RegistryRootKey PerformanceData =>
            new RegistryRootKey(Microsoft.Win32.Registry.PerformanceData);
        /// <summary>
        /// Root key for users
        /// </summary>
        public static RegistryRootKey Users =>
            new RegistryRootKey(Microsoft.Win32.Registry.Users);
    }
}
