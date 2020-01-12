using SFX.CSharp.ROP;
using SFX.CSharp.System.Windows.Model.Machine;
using SFX.CSharp.System.Windows.Model.Registry;
using System;
using static SFX.CSharp.ROP.Library;
using static System.Threading.Interlocked;

namespace SFX.CSharp.System.Infrastructure
{
    /// <summary>
    /// Implements <see cref="IMachineGuidReader"/>
    /// </summary>
    public sealed class MachineGuidReader : IMachineGuidReader
    {
        internal static readonly RegistryKeyName MachineGuidKeyName =
            new RegistryKeyName { Value = "MachineGuid" };
        internal static readonly RegistryKeyName DigitalMachineGuidKeyName =
            new RegistryKeyName { Value = "DigitalMachineGuid" };

        public MachineGuidReader(IRegistryReader registryReader) =>
            RegistryReader = registryReader ?? throw new ArgumentNullException(nameof(registryReader));

        internal IRegistryReader RegistryReader { get; }
        internal RegistrySubPath Path { get; private set; }

        internal long InitializationRunCount = 0L;
        /// <inheritdoc/>
        public void Initialize()
        {
            try
            {
                if (1L < Increment(ref InitializationRunCount))
                    return;

                var (success, path) =
                    RegistrySubPath.Create("Software",
                    "Microsoft",
                    "Cryptography");
                if (!success)
                    return;

                Path = path;

                Increment(ref InitializationCount);
            }
            catch { }
            finally
            {
                Decrement(ref InitializationRunCount);
            }
        }

        internal long InitializationCount = 0L;
        /// <inheritdoc/>
        public bool IsInitialized() => 0L < Read(ref InitializationCount);

        /// <inheritdoc/>
        public Result<MachineGuid> GetMachineGuid()
        {
            if (!IsInitialized())
                throw new InvalidOperationException("MachineGuidReader is not initialized");

            var (success, result, error) = RegistryReader
                .ReadLocalMachineStringValue(Path, MachineGuidKeyName);

            return success ?
                Succeed(new MachineGuid { Value = result.Value }) :
                Fail<MachineGuid>(error);
        }
    }
}
