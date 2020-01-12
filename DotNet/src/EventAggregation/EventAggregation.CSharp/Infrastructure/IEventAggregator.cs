using System;
using System.Threading;

namespace SFX.EventAggregation.Infrastructure
{
    /// <summary>
    /// Interface describing the capability to subscribe to, unsubscribe from and publish on a given in memory
    /// event bus
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of messages to publish</typeparam>
    public interface IEventAggregator<T>
    {
        /// <summary>
        /// Subscribes to messages
        /// </summary>
        /// <param name="subscriber">The subscriber</param>
        /// <param name="synchronizationContext">A <see cref="SynchronizationContext"/> via which the eventual notification should</param>
        /// <param name="serializeNotification">Flag telling whether notification should be serialized</param>
        /// <returns>The subscription id</returns>
        long Subscribe(IHandle<T> subscriber,
            SynchronizationContext synchronizationContext,
            bool serializeNotification);

        /// <summary>
        /// Subscribes to messages
        /// </summary>
        /// <param name="subscriber">The subscriber</param>
        /// <param name="synchronizationContext">A <see cref="SynchronizationContext"/> via which the eventual notification should</param>
        /// <param name="serializeNotification">Flag telling whether notification should be serialized</param>
        /// <returns>The subscription id</returns>
        long Subscribe(IHandleAsync<T> subscriber,
            SynchronizationContext synchronizationContext,
            bool serializeNotification);

        /// <summary>
        /// Unsubscribes 
        /// </summary>
        /// <param name="subscriptionId">The subscription id to unsubscribe</param>
        /// <returns>True if unsubscribe was successfull</returns>
        bool Unsubscribe(long subscriptionId);

        /// <summary>
        /// Publishes the provided <paramref name="message"/> to all subscribers
        /// </summary>
        /// <param name="message">The message to publish</param>
        void Publish(T message);
    }
}
