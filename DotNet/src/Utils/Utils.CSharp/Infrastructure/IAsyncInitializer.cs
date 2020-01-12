using System.Threading.Tasks;

namespace SFX.CSharp.Utils.Infrastructure
{
    /// <summary>
    /// Describes the capability of initializing "something" in an asynchronous manner
    /// </summary>
    public interface IAsyncInitializer
    {
        /// <summary>
        /// Performs the initialization asynchronously
        /// </summary>
        Task InitializeAsync();
    }
}
