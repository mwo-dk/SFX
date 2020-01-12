using System;

namespace SFX.EventAggregation.Infrastructure
{
    /// <summary>
    /// Factory interface to create event aggregators
    /// </summary>
    public interface IEventAggregatorFactory
    {
        /// <summary>
        /// Creates a new instance of an <see cref="IEventAggregator{T}"/>
        /// </summary>
        /// <returns>The newly created <see cref="IEventAggregator{T}"/></returns>
        /// <typeparam name="T">The <see cref="Type"/> of messages to publish</typeparam>
        IEventAggregator<T> Create<T>();
    }
}
