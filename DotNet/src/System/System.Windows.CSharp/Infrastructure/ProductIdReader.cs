using SFX.CSharp.ROP;
using SFX.CSharp.System.Windows.Model.Product;
using SFX.CSharp.System.Windows.Model.Registry;
using System;
using static SFX.CSharp.ROP.Library;
using static System.Threading.Interlocked;

namespace SFX.CSharp.System.Infrastructure
{
    /// <summary>
    /// Implements <see cref="IProductIdReader"/>
    /// </summary>
    public sealed class ProductIdReader : IProductIdReader
    {
        internal static readonly RegistryKeyName ProductIdKeyName =
            new RegistryKeyName { Value = "ProductId" };
        internal static readonly RegistryKeyName DigitalProductIdKeyName =
            new RegistryKeyName { Value = "DigitalProductId" };

        public ProductIdReader(IRegistryReader registryReader) =>
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
                    "Windows NT",
                    "CurrentVersion");
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
        public Result<ProductId> GetProductId()
        {
            if (!IsInitialized())
                throw new InvalidOperationException("ProductReader is not initialized");

            var (success, result, error) = RegistryReader
                .ReadLocalMachineStringValue(Path, ProductIdKeyName);

            return success ?
                Succeed(new ProductId { Value = result.Value }) :
                Fail<ProductId>(error);
        }

        /// <inheritdoc/>
        public Result<DigitalProductId> GetDigitalProductId()
        {
            if (!IsInitialized())
                throw new InvalidOperationException("ProductReader is not initialized");

            var (success, result, error) = RegistryReader
                .ReadLocalMachineBlobValue(Path, DigitalProductIdKeyName);

            return success ?
                Succeed(new DigitalProductId { Value = result.Value }) :
                Fail<DigitalProductId>(error);
        }
    }
}
