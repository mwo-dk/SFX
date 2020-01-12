namespace SFX.EventAggregation.Infrastructure
{
    /// <summary>
    /// Implements <see cref="IEventAggregatorFactory"/>
    /// </summary>
    public sealed class EventAggregatorFactory :
        IEventAggregatorFactory
    {
        /// <inheritdoc/>
        public IEventAggregator<T> Create<T>() =>
            new EventAggregator<T>();
    }
}
