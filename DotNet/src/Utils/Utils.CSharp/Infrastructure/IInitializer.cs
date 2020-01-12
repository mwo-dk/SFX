namespace SFX.Utils.Infrastructure
{
    /// <summary>
    /// Describes the capability of initializing "something"
    /// </summary>
    public interface IInitializer
    {
        /// <summary>
        /// Performs the initialization
        /// </summary>
        void Initialize();
    }
}
