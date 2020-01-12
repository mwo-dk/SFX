using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using static System.Threading.Interlocked;

namespace SFX.EventAggregation.Infrastructure
{
    /// <summary>
    /// Implements <see cref="IEventAggregator{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of messages this bus handles</typeparam>
    public sealed class EventAggregator<T> : IEventAggregator<T>
    {
        internal long NewSubscriptionId;
        internal ConcurrentDictionary<long, ActionBlock<T>> Subscriptions =
            new ConcurrentDictionary<long, ActionBlock<T>>();

        /// <inheritdoc/>
        public long Subscribe(IHandle<T> subscriber,
            SynchronizationContext synchronizationContext = null,
            bool serializeNotification = false)
        {
            var id = Increment(ref NewSubscriptionId);
            Subscriptions[id] = CreateActionBlock(subscriber,
                synchronizationContext,
                serializeNotification);
            return id;
        }

        /// <inheritdoc/>
        public long Subscribe(IHandleAsync<T> subscriber,
            SynchronizationContext synchronizationContext = null,
            bool serializeNotification = false)
        {
            var id = Increment(ref NewSubscriptionId);
            Subscriptions[id] = CreateActionBlock(subscriber,
                synchronizationContext,
                serializeNotification);
            return id;
        }

        /// <inheritdoc/>
        public bool Unsubscribe(long subscriptionId)
        {
            var result = Subscriptions.TryRemove(subscriptionId, out ActionBlock<T> handler);
            if (result)
                try
                {
                    handler.Complete();
                }
                catch { }
            return result;
        }

        /// <inheritdoc/>
        public void Publish(T message)
        {
            var handlers = Subscriptions.Values.ToArray();
            foreach (var handler in handlers)
                handler.Post(message);
        }

        private static ActionBlock<T> CreateActionBlock(IHandle<T> subscriber,
            SynchronizationContext synchronizationContext,
            bool serializeNotification)
        {
            var options = new ExecutionDataflowBlockOptions();
            if (serializeNotification)
                options.MaxDegreeOfParallelism = 1;
            var reference = new WeakReference(subscriber);
            return new ActionBlock<T>(message =>
            {
                void DoPublish(object _)
                {
                    try
                    {
                        var target = reference.Target as IHandle<T>;
                        if (target is null)
                            return;
                        target.Handle(message);
                    }
                    catch
                    {
                    }
                }
                if (synchronizationContext == null)
                    DoPublish(null);
                else synchronizationContext.Post(DoPublish, null);
            }, options);
        }

        private static ActionBlock<T> CreateActionBlock(IHandleAsync<T> subscriber,
            SynchronizationContext synchronizationContext,
            bool serializeNotification)
        {
            var options = new ExecutionDataflowBlockOptions();
            if (serializeNotification)
                options.MaxDegreeOfParallelism = 1;
            var reference = new WeakReference(subscriber);
            return new ActionBlock<T>(message =>
            {
                async void DoPublish(object _)
                {
                    try
                    {
                        var target = reference.Target as IHandleAsync<T>;
                        if (target is null)
                            return;
                        await target.HandleAsync(message)
                        .ConfigureAwait(false);
                    }
                    catch
                    {
                    }
                }
                if (synchronizationContext == null)
                    DoPublish(null);
                else synchronizationContext.Post(DoPublish, null);
            }, options);
        }
    }
}
