using SFX.CSharp.ROP;
using SFX.CSharp.System.Windows.Model.Product;
using SFX.CSharp.Utils.Infrastructure;

namespace SFX.CSharp.System.Infrastructure
{
    /// <summary>
    /// Interface describing the capability of reading the Windows installation product ids.
    /// </summary>
    public interface IProductIdReader : IInitializable
    {
        /// <summary>
        /// Reads the Windows product id from registry
        /// </summary>
        /// <returns></returns>
        Result<ProductId> GetProductId();

        /// <summary>
        /// Reads the Windows digital product id from registry
        /// </summary>
        /// <returns></returns>
        Result<DigitalProductId> GetDigitalProductId();
    }
}
