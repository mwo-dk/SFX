# SFX.EventAggregation
Simple event aggregator library. This repo contains two libraries:

* A C# library, which can be fetched on nuget: [SFX.EventAggregation.CSharp](https://www.nuget.org/packages/SFX.EventAggregation.CSharp/).
* And an F# library, which can be fetched on nuget: [SFX.EventAggregation](https://www.nuget.org/packages/SFX.EventAggregation/).

The library simply facilitates strongly typed event aggregators, that:

* Lets entities subscribe and unsubscribe to typed events in a synchronous and asynchronous manner, meaning it is an in process event/message bus.
* Lets entities publish events on the bus.

An event aggregator instance is basically a holder of weak references to subscribers who care about certain types of messages. Weak references are in use in order to let subscribers die silently without having to clean themselves up nicely.

The typical usage of an event aggregator is in long-running processes and/or applications with a UI, where events happen and need to be reacted upon.

## Usage C#

### EventAggregator<>

The class ```EventAggregator<>``` implements the following contract:

``` csharp
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
```

and can simply by instantiated and utilized in this manner:

``` csharp
public class Source {
    public Source(IEventAggregator<Message> eventAggregator) =>
        EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

    internal IEventAggregator<Message> EventAggregator {get;}

    public void DoStuffAndLetThemKnow() {
        ...
        var message = CreateTheMessage();
        EventAggregator.Publish(message);
        ...
    }
}

public class Sink : IHandle<Message>, IInitializable, IDisposable {
    public Sink(IEventAggregator<Message> eventAggregator) =>
        EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

    internal IEventAggregator<Message> EventAggregator {get;}
    internal long Subscription {get;set;}

    public void Initialize() {
        ...
        Subscription = EventAggregator.Subscribe(this);
    }

    public void Handle(Message message) {
        ...
        // React on stuff - don't throw exceptions
        ...
    }

    public void Dispose() => EventAggregator.UnSubscribe(Subscription);
}

public class AsyncSink : IHandleAsync<Message>, IInitializable, IDisposable {
    public AsyncSink(IEventAggregator<Message> eventAggregator) =>
        EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

    internal IEventAggregator<Message> EventAggregator {get;}
    internal long Subscription {get;set;}

    public void Initialize() {
        ...
        Subscription = EventAggregator.Subscribe(this);
    }

    public Task HandleAsync(Message message) {
        ...
        // React on stuff - don't throw exceptions
        ...
    }

    public void Dispose() => EventAggregator.UnSubscribe(Subscription);
}
```

As can be seen, this is a very basic way to hook up parts of your application - and many implementations can be found elsewhere.

```EventAggregator``` is an implementation of ```IEventAggregator<>```, and the methods are:

* ```Subscribe(IHandle<T> subscriber, SynchronizationContext synchronizationContext = null, bool serializeNotification = false) -> long```, that is the ```subscriber``` implements ```IHandle<T>``` (that is a single method with signature ```Handle(T message) -> void```). Upon subscription, the subscriber can provide a ```SynchronizationContext```, which is helpful ie. in a UI application. The parameter ```serializeNotification``` denotes whether notifications should be fired asap or queued and fired in order. The method returns a ```long```, which is the ticket/subscription id, which is utilized upon unsubscription.
* ```Subscribe(IHandleAsync<T> subscriber, SynchronizationContext synchronizationContext = null, bool serializeNotification = false) -> long```, is similar to the previous method, besides that the ```subscriber``` implements ```IHandleAsync<T>``` (that is a single method with signature ```HandleAsync(T message) -> Task```). 
* ```Unsubscribe(long subscriptionId) -> bool```, removes the subscription denoted by ```subscriptionId```. The method returns true if unsubscription succeeded, false otherwise. 
* ```Publish(T message) -> void```, simply publishes messages to all subscribers in the manner, that the subscription was set up.

### EventAggregatorRepository<>

Implements the following contract:

``` csharp
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
```

and is basically a holder of named and unnamed ```EventAggregator<>```s. It is recommended to rely in this in larger applications, that is to inject an ```IEventAggregatorRepository``` instead of individual ```IEventAggregator<>```s. Doing it this way (and ie. having the ```IEventAggregatorRepository```) being a global instance (singleton) you can ensure that all relevant parties talk via the same bus:

``` csharp
public class Source : , IInitializable {
    public Source(IEventAggregatorRepository eventAggregatorRepository) =>
        EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregatorRepository));

    internal IEventAggregatorRepository EventAggregatorRepository {get;}
    internal IEventAggregator<Message> EventAggregator {get; private set;}

    public void Initialize() {
        ...
        EventAggregator = EventAggregatorRepository.GetEventAggregator<Message>();
    }

    public void DoStuffAndLetThemKnow() {
        ...
        var message = CreateTheMessage();
        EventAggregator.Publish(message);
        ...
    }
}

public class Sink : IHandle<Message>, IInitializable, IDisposable {
    public Sink(IEventAggregatorRepository eventAggregatorRepository) =>
        EventAggregatorRepository = eventAggregatorRepository ?? throw new ArgumentNullException(nameof(eventAggregatorRepository));

    internal IEventAggregatorRepository EventAggregatorRepository {get;}
    internal IEventAggregator<Message> EventAggregator {get; private set;}
    internal long Subscription {get;set;}

    public void Initialize() {
        ...
        EventAggregator = EventAggregatorRepository.GetEventAggregator<Message>();
        Subscription = EventAggregator.Subscribe(this);
    }

    public void Handle(Message message) {
        ...
        // React on stuff - don't throw exceptions
        ...
    }

    public void Dispose() => EventAggregator.UnSubscribe(Subscription);
}

public class AsyncSink : IHandleAsync<Message>, IInitializable, IDisposable {
    public AsyncSink(IEventAggregatorRepository eventAggregatorRepository) =>
        EventAggregator = eventAggregatorRepository ?? throw new ArgumentNullException(nameof(eventAggregatorRepository));

    internal IEventAggregatorRepository EventAggregatorRepository {get;}
    internal IEventAggregator<Message> EventAggregator {get; private set;}
    internal long Subscription {get;set;}

    public void Initialize() {
        ...
        EventAggregator = EventAggregatorRepository.GetEventAggregator<Message>();
        Subscription = EventAggregator.Subscribe(this);
    }

    public Task HandleAsync(Message message) {
        ...
        // React on stuff - don't throw exceptions
        ...
    }

    public void Dispose() => EventAggregator.UnSubscribe(Subscription);
}
```
The notion of having named and unnamed versions of the ```GetEventAggregator<T>()``` methods only is due to a personal  (maybe mis-design) need to have different buses for same types of events.

## Usage F#

An F# wrapping library has also been provided for. It basically wraps around the C# implementation and:

* Provides simple utility functions to create, subscribe, unsubscibe and publish messages. As mentioned above, creating individual buses is not recommended. It is simpler to utilize a shared instance accross the application.
* Provides support for utilizing basic F# functions as subscribers.

### Utilizing event aggregators

The module defines the following functions:

* ```createEventAggregator: unit -> EventAggregator<'a>```, is a simple replacement of the C# constructor.ff
* ```subscribe: bool -> Subscriber<'a> -> IEventAggregator<'a> -> Result<Subscriber, EventAggregatorError>``` is similar to the OO-scenario in the C# usage section with the extension that:
    - ```Subscriber<'a>``` is a union case, that embraces all versions of synchronous and asynchronous interfaces or functions as well as with or without ```SynchronizationContext```es. This type is introduced below.
    - We utilize railway oriented programming, as denoted by [Scott Wlaschin](https://fsharpforfunandprofit.com/posts/recipe-part2/).
* ```unsubscribe: Subscriber -> IEventAggregator<'a> -> Result<unit, EventAggregatorError>``` simply unsubscribes with the ticket provided previously.
* ```publish: 'a -> IEventAggregator<'a> -> unit```, simply publishes the provided message over the given event aggregator.

#### Extra types introduced

In order to do the above, the following types and functions have been added:

``` fsharp
type SyncHandler<'a> = 'a -> unit
type AsyncHandler<'a> = 'a -> Async<unit>
type Subscriber<'a> =
| Sync of IHandle<'a>
| SyncWithContext of IHandle<'a>*SynchronizationContext
| Async of IHandleAsync<'a>
| AsyncWithContext of IHandle<'a>*SynchronizationContext
| SyncHandler of SyncHandler<'a>
| SyncHandlerWithContext of SyncHandler<'a>*SynchronizationContext
| AsyncHandler of AsyncHandler<'a>
| AsyncHandlerWithContext of AsyncHandler<'a>*SynchronizationContext
type Subscription = int64
type EventAggregatorError =
| LockNotAcquired
| EventAggregatorNotFound
| SubscriptionNotFound of Subscription
```
Where:
* ```SyncHandler<'a>``` is just an alias for a function taking an argument and returning ```unit```, that is the most basic handler.
* ```AsyncHandler<'a>``` is just an alias for a function taking an argument and returning ```Async<unit>```, that is the most basic asynchronous handler.
* ```Subscriber<'a>````, is the abovementioned sum type representing all accepted ways we support / extend the shape of a subscriber and its utilization. 
* ```Subscription```, is just an alias for the ticket - a 64 bit signed integer - for a subscription.
* ```EventAggregatorError```, is the sum type representing ways invoking the various functions can fail.

A few helper functions, to create the non-OO cases for ```Subscriber<'a>``` have been provided for:

``` fsharp
let sync = SyncHandler
let syncWithContext x y = SyncHandlerWithContext(x,y)
let async = AsyncHandler
let asyncWithContext x y = AsyncHandlerWithContext(x,y)
```
As mentioned, this library sits on top of, and can therefore seamlessly work together with other libraries utilizing the simpler C# version. The way functions have been lifted in is via types, that implement ```IHandle<'a>``` and ```IHandleAsync<'a>``` respectively. These two types take ```SyncHandler<'a>```'s / ```AsyncHandler<'a>```'s in there constructors. When subscription occurs on a given event aggregator, an instance, that takes the function handler is instantiated and stowed away (in order not to be GC'ed), and kept alive till no longer needed.

### Working with event aggregator repositories

As mentioned above, if this type of device is required in your monolith, it is recommended to utilize ```IEventAggregatorRepository```, which can be created, initialized and checked via the functions:

* ```createEventAggregatorRepository: unit -> EventAggregatorRepository```
* ```initializeEventAggregatorRepository: IEventAggregatorRepository -> unit```
* ```isEventAggregatorRepositoryInitialized: IEventAggregatorRepository -> bool```

As mentioned above, getting (and creating if not found) an event aggregator is done in a named and an unnamed fashion. In this library, it is done via:

* ```getEventAggregator: EventAggregatorInstance -> IEventAggregatorRepository -> IEventAggregator<'a>```

where ```EventAggregatorInstance``` denoets whether a named or an unnamed instance is requested:
``` fsharp
type EventAggregatorInstance =
| Anonymous
| Named of string
let named = Named
```
