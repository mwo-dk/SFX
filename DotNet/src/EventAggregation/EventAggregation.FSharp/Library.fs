module SFX.EventAggregation

open System
open System.Threading
open SFX.ROP
open SFX.EventAggregation.Model
open SFX.EventAggregation.Infrastructure

/// Creates an EventAggregator<>
let createEventAggregator() = new EventAggregator<'a>()

/// Alias for: 'a -> unit
type SyncHandler<'a> = 'a -> unit
/// Alias for : 'a -> Async<unit>
type AsyncHandler<'a> = 'a -> Async<unit>
/// Represents a subscriber
type Subscriber<'a> =
/// The subscriber is an IHandle<>
| Sync of IHandle<'a>
/// The subscriber is an IHandle<> and requires dispatching via a SynchronizationContext
| SyncWithContext of IHandle<'a>*SynchronizationContext
/// The subscriber is an IHandleAsync<>
| Async of IHandleAsync<'a>
/// The subscriber is an IHandleAsync<> and requires dispatching via a SynchronizationContext
| AsyncWithContext of IHandleAsync<'a>*SynchronizationContext
/// The subscriber is an SyncHandler<>
| SyncHandler of SyncHandler<'a>
/// The subscriber is an SyncHandler<> and requires dispatching via a SynchronizationContext
| SyncHandlerWithContext of SyncHandler<'a>*SynchronizationContext
/// The subscriber is an AsyncHandler<>
| AsyncHandler of AsyncHandler<'a>
/// The subscriber is an AsyncHandler<> and requires dispatching via a SynchronizationContext
| AsyncHandlerWithContext of AsyncHandler<'a>*SynchronizationContext
/// Creates a Subscriber from a SyncHandler<>
let sync = SyncHandler
/// Creates a Subscriber from a SyncHandler and a SynchronizationContext
let syncWithContext x y = SyncHandlerWithContext(x,y)
/// Creates a Subscriber from an AsyncHandler<>
let async = AsyncHandler
/// Creates a Subscriber from an AsyncHandler and a SynchronizationContext
let asyncWithContext x y = AsyncHandlerWithContext(x,y)

/// Represents a subscription id - alias to int64
type Subscription = int64

/// Represents errors, that can occur when working with EventAggregators
type EventAggregatorError =
/// The lock was not acquired when subscribing or unubscribing
| LockNotAcquired
/// Occurs when trying to unsubscribe and the actual inner subscription was not found
| EventAggregatorNotFound
/// The denoted subscription was not found
| SubscriptionNotFound of Subscription

/// Represents a synchronous handle object from a SyncHandler<>
type Handle<'a>(handler: SyncHandler<'a>) =
    interface IHandle<'a> with
        member _.Handle message = message |> handler
/// Creates a Handle<>
let createHandle x = Handle(x)
/// Represents an asynchronous handle object from a SyncHandler<>
type AsyncHandle<'a>(handler: AsyncHandler<'a>) =
    interface IHandleAsync<'a> with
        member _.HandleAsync message = 
            Tasks.Task.Run(fun () -> 
                message |> handler |> Async.RunSynchronously)
/// Creates an AsyncHandle<>
let createAsyncHandle x = AsyncHandle(x)

// This tiny infrastructure is in order to hold on to else temporary objects created to be
// registered inside event aggregators - in the functional cases.

let mutable private lock = SpinLock()
let mutable private hiddenEntities : (WeakReference * Map<Subscription, obj>) list = List.empty
let private addHiddenEntity id x evtAgg =
    let mutable lockTaken = false
    try 
        lock.TryEnter(&lockTaken)
        if lockTaken |> not then
            LockNotAcquired |> fail
        else
            // Clean dead evt aggs
            hiddenEntities <- hiddenEntities |> List.filter (fun (e,_) -> e.Target <> null)
            match hiddenEntities |> List.tryFind (fun (e, _) -> obj.ReferenceEquals(e.Target, evtAgg)) with
            | Some slot -> 
                let e, m = slot
                let m = m |> Map.add id (x :> obj)
                hiddenEntities <- (e,m)::(hiddenEntities |> List.except [slot])
            | _ ->
                let m = Map.empty |> Map.add id (x :> obj)
                hiddenEntities <- (WeakReference(evtAgg),m)::hiddenEntities
            () |> succeed
    finally
        if lockTaken then
            lock.Exit(false)
let private removeHiddenEntity id evtAgg =
    let mutable lockTaken = false
    try 
        lock.TryEnter(&lockTaken)
        if lockTaken |> not then
            LockNotAcquired |> fail
        else 
            // Clean dead evt aggs
            hiddenEntities <- hiddenEntities |> List.filter (fun (e,_) -> e.Target <> null)
            match hiddenEntities |> List.tryFind (fun (e, _) -> obj.ReferenceEquals(e.Target, evtAgg)) with
            | Some slot -> 
                let e, m = slot
                let m = m |> Map.remove id
                hiddenEntities <- (e,m)::(hiddenEntities |> List.except [slot])
                () |> succeed
            | _ -> EventAggregatorNotFound |> fail
    finally
        if lockTaken then
            lock.Exit(false)

/// Subscribes the handler to the evtAgg (IEventAggregator<>). The flag
/// serialize denotes whether to serialize messages published
let subscribe serialize handler (evtAgg: IEventAggregator<'a>) =
    let doSyncHandler x s =
        let x = Handle(x)
        let id = evtAgg.Subscribe(x, s, serialize)
        match evtAgg |> addHiddenEntity id x with
        | Success _ -> id |> succeed
        | Failure error -> error |> fail
    let doAsyncHandler x s =
        let x = AsyncHandle(x)
        let id = evtAgg.Subscribe(x, s, serialize)
        match evtAgg |> addHiddenEntity id x with
        | Success _ -> id |> succeed
        | Failure error -> error |> fail
    match handler with
    | Sync x -> evtAgg.Subscribe(x, null, serialize) |> succeed
    | SyncWithContext (x,s) -> evtAgg.Subscribe(x, s, serialize) |> succeed
    | Async x -> evtAgg.Subscribe(x, null, serialize) |> succeed
    | AsyncWithContext (x,s) -> evtAgg.Subscribe(x, s, serialize) |> succeed
    | SyncHandler x -> doSyncHandler x null
    | SyncHandlerWithContext (x, s) -> doSyncHandler x s
    | AsyncHandler x -> doAsyncHandler x null
    | AsyncHandlerWithContext (x, s) -> doAsyncHandler x s
      
/// Removes the subscription denoted by id from evtAgg (IEventAggregator<>)
let unsubscribe id (evtAgg: IEventAggregator<'a>) =
    if evtAgg.Unsubscribe id then
        match evtAgg |> removeHiddenEntity id with
        | Success _ -> () |> succeed
        | Failure error ->
            match error with
            | LockNotAcquired -> LockNotAcquired |> fail
            | _ -> () |> succeed
    else id |> SubscriptionNotFound |> fail

/// Publishes the message via evtAgg (IEventAggregator<>)
let publish message (evtAgg: IEventAggregator<'a>) = evtAgg.Publish message

/// Creates an EventAggregatorRepository
let createEventAggregatorRepository() =
    EventAggregatorRepository()
/// Initializes an IEventAggregatorRepository
let initializeEventAggregatorRepository (repo: IEventAggregatorRepository) =
    repo.Initialize()
/// Checkes whether an IEventAggregatorRepository is initialized
let isEventAggregatorRepositoryInitialized (repo: IEventAggregatorRepository) =
    repo.IsInitialized()

/// Represents a "kind" of event aggregator. Anonymous means "the one" denoted
/// by type only. Named means a dedicated "bus" with a given type
type EventAggregatorInstance =
| Anonymous
| Named of string
/// Creates the name
let named = Named
/// Gets an IEventAggregator from repo (IEventAggregatorRepository). If instance
/// is Anonymous the default instance is returned, else the one denoted by the name
let getEventAggregator instance (repo: IEventAggregatorRepository) = 
    match instance with
    | Anonymous -> repo.GetEventAggregator()
    | Named name -> repo.GetEventAggregator(Name(name))