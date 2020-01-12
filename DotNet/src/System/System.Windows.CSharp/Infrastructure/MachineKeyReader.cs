using SFX.CSharp.ROP;
using SFX.CSharp.System.Model;
using System;
using static SFX.CSharp.ROP.Library;

namespace SFX.CSharp.System.Infrastructure
{
    /// <summary>
    /// Implements <see cref="IMachineKeyReader"/>
    /// </summary>
    public sealed class MachineKeyReader : IMachineKeyReader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="machineGuidReader">The <see cref="IMachineGuidReader"/> utilized</param>
        public MachineKeyReader(IMachineGuidReader machineGuidReader) =>
            MachineGuidReader = machineGuidReader ?? throw new ArgumentNullException(nameof(machineGuidReader));

        internal IMachineGuidReader MachineGuidReader { get; }

        public Result<MachineKey> GetMachineKey()
        {
            try
            {
                var (success, machineGuid, error) = MachineGuidReader.GetMachineGuid();
                if (!success)
                    return Fail<MachineKey>(error);
                success = Guid.TryParse(machineGuid.Value, out Guid guid);
                if (!success)
                    return Fail<MachineKey>(new InvalidCastException());
                return Succeed(new MachineKey { Value = guid.ToByteArray() });
            }
            catch (Exception exn)
            {
                return Fail<MachineKey>(exn);
            }
        }
    }
}
