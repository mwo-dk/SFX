using System.Threading.Tasks;

namespace SFX.Utils.Infrastructure
{
    /// <summary>
    /// Describes the capability of being initializable in an asynchronous manner
    /// </summary>
    public interface IAsyncInitializable
    {
        /// <summary>
        /// Initializes the object asynchronously
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Determines whether the instance has been initialized
        /// </summary>
        /// <returns>True if the object has been initialized, else false</returns>
        bool IsInitialized();
    }
}
