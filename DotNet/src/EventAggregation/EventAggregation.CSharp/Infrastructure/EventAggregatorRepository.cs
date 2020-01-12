using SFX.EventAggregation.Model;
using System;
using System.Collections.Concurrent;
using System.Threading;
using static System.Threading.Interlocked;

namespace SFX.EventAggregation.Infrastructure
{
    /// <summary>
    /// Implements <see cref="IEventAggregatorRepository"/>
    /// </summary>
    public sealed class EventAggregatorRepository : IEventAggregatorRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="factory">The utilized <see cref="IEventAggregatorFactory"/></param>
        internal EventAggregatorRepository(IEventAggregatorFactory factory) =>
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));

        /// <summary>
        /// Default constructor
        /// </summary>
        public EventAggregatorRepository() : this(new EventAggregatorFactory()) { }

        internal IEventAggregatorFactory Factory { get; }

        internal ConcurrentDictionary<Type, Lazy<object>> UnNamedMap { get; private set; }
        internal ConcurrentDictionary<TypeAndName, Lazy<object>> NamedMap { get; private set; }

        /// <inheritdoc/>
        public void Initialize()
        {
            if (IsInitialized())
                return;

            UnNamedMap = new ConcurrentDictionary<Type, Lazy<object>>();
            NamedMap = new ConcurrentDictionary<TypeAndName, Lazy<object>>();

            Increment(ref InitializationCount);
        }

        internal long InitializationCount;
        /// <inheritdoc/>
        public bool IsInitialized() => 0L != Read(ref InitializationCount);

        /// <inheritdoc/>
        public IEventAggregator<T> GetEventAggregator<T>()
        {
            if (!IsInitialized())
                throw new InvalidOperationException("EventRepository is not initialized");

            var result = UnNamedMap.GetOrAdd(typeof(T),
                _ => new Lazy<object>(() =>
                {
                    var eventAggregator = Factory.Create<T>();
                    return eventAggregator as object;
                }, LazyThreadSafetyMode.ExecutionAndPublication)).Value;
            return result as IEventAggregator<T>;
        }

        /// <inheritdoc/>
        public IEventAggregator<T> GetEventAggregator<T>(Name name)
        {
            if (!IsInitialized())
                throw new InvalidOperationException("EventRepository is not initialized");
            if (!name.IsValid())
                throw new ArgumentException($"Name \"{name}\" is not valid");

            var result = NamedMap.GetOrAdd(new TypeAndName(typeof(T), name),
                _ => new Lazy<object>(() =>
                {
                    var eventAggregator = Factory.Create<T>();
                    return eventAggregator as object;
                }, LazyThreadSafetyMode.ExecutionAndPublication)).Value;
            return result as IEventAggregator<T>;
        }
    }
}