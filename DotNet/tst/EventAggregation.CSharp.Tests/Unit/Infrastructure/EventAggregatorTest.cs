using SFX.EventAggregation.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SFX.EventAggregation.Test.Unit.Infrastructure
{
    public sealed class Handler : IHandle<int>
    {
        public ManualResetEvent Event { get; } = new ManualResetEvent(false);
        public int ReceivedValue { get; private set; }

        public void Handle(int message)
        {
            ReceivedValue = message;
            Event.Set();
        }
    }

    public sealed class AsyncHandler : IHandleAsync<int>
    {
        public ManualResetEvent Event { get; } = new ManualResetEvent(false);
        public int ReceivedValue { get; private set; }

        public Task HandleAsync(int message) => Task.Run(() =>
        {
            ReceivedValue = message;
            Event.Set();
        });
    }

    [Trait("Category", "Unit")]
    public sealed class EventAggregatorTest
    {
        private readonly Handler _handler = new Handler();
        private readonly AsyncHandler _asyncHandler = new AsyncHandler();

        #region Type test
        [Fact]
        public void EventAggregator_Is_Generic() =>
            Assert.True(typeof(EventAggregator<>).IsGenericTypeDefinition);

        [Fact]
        public void EventAggregator_Implements_IEventAggregator() =>
            Assert.True(typeof(IEventAggregator<int>).IsAssignableFrom(typeof(EventAggregator<int>)));
        #endregion

        #region Subscribe (sync)
        [Fact]
        public void Subscribe_Sync_Adds_Item_To_Subscriptions()
        {
            var sut = Create();

            var result = sut.Subscribe(_handler);

            Assert.Equal(1, result);
            Assert.Single(sut.Subscriptions);
        }
        #endregion 

        #region Subscribe (async)
        [Fact]
        public void Subscribe_Async_Adds_Item_To_Subscriptions()
        {
            var sut = Create();

            var result = sut.Subscribe(_asyncHandler);

            Assert.Equal(1, result);
            Assert.Single(sut.Subscriptions);
        }
        #endregion

        #region Subscribe (async)
        [Fact]
        public void Unsubscribe_Works()
        {
            var sut = Create();

            var result = sut.Subscribe(_asyncHandler);
            var ok = sut.Unsubscribe(result);

            Assert.Equal(1, result);
            Assert.Empty(sut.Subscriptions);
            Assert.True(ok);
        }
        #endregion 

        #region Publish (sync)
        [Fact]
        public void Publish_Sync_Works()
        {
            var sut = Create();
            sut.Subscribe(_handler);

            sut.Publish(666);
            _handler.Event.WaitOne();

            Assert.Equal(666, _handler.ReceivedValue);
        }
        #endregion

        #region Publish (async)
        [Fact]
        public void Publish_Async_Works()
        {
            var sut = Create();
            sut.Subscribe(_asyncHandler);

            sut.Publish(666);
            _asyncHandler.Event.WaitOne();

            Assert.Equal(666, _asyncHandler.ReceivedValue);
        }
        #endregion

        #region Utility
        private EventAggregator<int> Create() =>
            new EventAggregator<int>();
        #endregion
    }

    public class Animal { }
    public class Mammal : Animal { }

    public sealed class AnimalHandler : IHandle<Animal>
    {
        public ManualResetEvent Event { get; } = new ManualResetEvent(false);
        public Animal ReceivedValue { get; private set; }

        public void Handle(Animal message)
        {
            ReceivedValue = message;
            Event.Set();
        }
    }

    public sealed class AsyncAnimalHandler : IHandleAsync<Animal>
    {
        public ManualResetEvent Event { get; } = new ManualResetEvent(false);
        public Animal ReceivedValue { get; private set; }

        public Task HandleAsync(Animal message) => Task.Run(() =>
        {
            ReceivedValue = message;
            Event.Set();
        });
    }

    [Trait("Category", "Unit")]
    public sealed class EventAggregatorCoVarianceTest
    {
        private readonly AnimalHandler _handler = new AnimalHandler();
        private readonly AsyncAnimalHandler _asyncHandler = new AsyncAnimalHandler();
        private readonly Animal _animal = new Animal();
        private readonly Mammal _mammal = new Mammal();

        #region Publish (sync)
        [Fact]
        public void Publish_Anomal_Sync_Works()
        {
            var sut = Create();
            sut.Subscribe(_handler);

            sut.Publish(_animal);
            _handler.Event.WaitOne();

            Assert.Same(_animal, _handler.ReceivedValue);
        }

        [Fact]
        public void Publish_Mamal_Sync_Works()
        {
            var sut = Create();
            sut.Subscribe(_handler);

            sut.Publish(_mammal);
            _handler.Event.WaitOne();

            Assert.Same(_mammal, _handler.ReceivedValue);
        }
        #endregion

        #region Publish (async)
        [Fact]
        public void Publish_Anomal_Async_Works()
        {
            var sut = Create();
            sut.Subscribe(_asyncHandler);

            sut.Publish(_animal);
            _asyncHandler.Event.WaitOne();

            Assert.Same(_animal, _asyncHandler.ReceivedValue);
        }

        [Fact]
        public void Publish_Mammal_Async_Works()
        {
            var sut = Create();
            sut.Subscribe(_asyncHandler);

            sut.Publish(_mammal);
            _asyncHandler.Event.WaitOne();

            Assert.Same(_mammal, _asyncHandler.ReceivedValue);
        }
        #endregion

        #region Utility
        private EventAggregator<Animal> Create() =>
            new EventAggregator<Animal>();
        #endregion
    }
}
