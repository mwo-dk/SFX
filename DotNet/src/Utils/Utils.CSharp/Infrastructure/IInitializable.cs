namespace SFX.Utils.Infrastructure
{
    /// <summary>
    /// Describes the capability of being initializable
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Initializes the object
        /// </summary>
        void Initialize();

        /// <summary>
        /// Determines whether the instance has been initialized
        /// </summary>
        /// <returns>True if the object has been initialized, else false</returns>
        bool IsInitialized();
    }
}
