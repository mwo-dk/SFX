using SFX.EventAggregation.Model;
using SFX.Utils.Infrastructure;
using System;

namespace SFX.EventAggregation.Infrastructure
{
    /// <summary>
    /// Interface to supply event aggregators
    /// </summary>
    public interface IEventAggregatorRepository : IInitializable
    {
        /// <summary>
        /// Gets an unnamed <see cref="IEventAggregator{T}"/>
        /// </summary>
        /// <returns>The <see cref="IEventAggregator{T}"/></returns>
        /// <typeparam name="T">The <see cref="Type"/> of messages to publish</typeparam>
        IEventAggregator<T> GetEventAggregator<T>();
        /// <summary>
        /// Gets a named <see cref="IEventAggregator{T}"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The <see cref="IEventAggregator{T}"/></returns>
        /// <typeparam name="T">The <see cref="Type"/> of messages to publish</typeparam>
        IEventAggregator<T> GetEventAggregator<T>(Name name);
    }
}
