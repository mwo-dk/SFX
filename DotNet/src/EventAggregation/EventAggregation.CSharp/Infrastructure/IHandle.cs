using System;

namespace SFX.EventAggregation.Infrastructure
{
    /// <summary>
    /// Interface describing the capability to handle messages of <see cref="Type"/> <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of message to handle</typeparam>
    public interface IHandle<in T>
    {
        /// <summary>
        /// The actual handler method
        /// </summary>
        /// <param name="message">The message to handle</param>
        void Handle(T message);
    }
}
