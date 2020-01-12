using AutoFixture;
using FakeItEasy;
using SFX.EventAggregation.Infrastructure;
using SFX.EventAggregation.Model;
using SFX.Utils.Infrastructure;
using System;
using Xunit;
using static FakeItEasy.A;

namespace SFX.EventAggregation.Test.Unit.Infrastructure
{
    [Trait("Category", "Unit")]
    public sealed class EventAggregatorRepositoryTest
    {
        private readonly IFixture _fixture;
        private readonly IEventAggregator<int> _aggregator;
        private readonly IEventAggregatorFactory _factory;
        private readonly Name _name;

        public EventAggregatorRepositoryTest()
        {
            _fixture = new Fixture().Customize(new SupportMutableValueTypesCustomization());

            _aggregator = Fake<IEventAggregator<int>>();
            _factory = Fake<IEventAggregatorFactory>();
            CallTo(() => _factory.Create<int>()).Returns(_aggregator);
            _name = _fixture.Create<Name>();
        }

        #region Type test
        [Fact]
        public void EventAggregatorRepository_Implements_IEventAggregatorRepository() =>
            Assert.True(typeof(IEventAggregatorRepository).IsAssignableFrom(typeof(EventAggregatorRepository)));

        [Fact]
        public void EventAggregatorRepository_Implements_IInitializable() =>
            Assert.True(typeof(IInitializable).IsAssignableFrom(typeof(EventAggregatorRepository)));

        [Fact]
        public void EventAggregatorRepository_Is_Sealed() =>
            Assert.True(typeof(EventAggregatorRepository).IsSealed);
        #endregion

        #region Initialization test
        [Fact]
        public void Initializing_With_Null_EventAggregatorFactory_Throws() =>
            Assert.Throws<ArgumentNullException>(() => new EventAggregatorRepository(null));

        [Fact]
        public void Initializing_Sets_Factory()
        {
            var sut = Create();

            Assert.Same(_factory, sut.Factory);
        }
        #endregion

        #region Initialize
        [Fact]
        public void Initializing_Initializes_UnNamed_Dictionary()
        {
            var sut = Create();
            sut.Initialize();

            Assert.NotNull(sut.UnNamedMap);
        }

        [Fact]
        public void Initializing_Initializes_Named_Dictionary()
        {
            var sut = Create();
            sut.Initialize();

            Assert.NotNull(sut.NamedMap);
        }
        #endregion

        #region IsInitialized
        [Fact]
        public void IsInitialized_Returns_False_When_Not_Initalized()
        {
            var sut = Create();

            Assert.False(sut.IsInitialized());
        }

        [Fact]
        public void IsInitialized_Returns_True_When_Initalized()
        {
            var sut = Create();
            sut.InitializationCount = 1L;

            Assert.True(sut.IsInitialized());
        }
        #endregion

        #region Get (un-named)
        [Fact]
        public void Get_Uninitalized_Throws()
        {
            var sut = Create();

            Assert.Throws<InvalidOperationException>(() => sut.GetEventAggregator<int>());
        }

        [Fact]
        public void Get_Creates()
        {
            var sut = Create();
            sut.Initialize();

            sut.GetEventAggregator<int>();

            CallTo(() => _factory.Create<int>()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Get_Returns()
        {
            var sut = Create();
            sut.Initialize();

            var result = sut.GetEventAggregator<int>();

            Assert.Same(_aggregator, result);
        }

        [Fact]
        public void Get_Returns_Same()
        {
            var sut = Create();
            sut.Initialize();

            var result1 = sut.GetEventAggregator<int>();
            var result2 = sut.GetEventAggregator<int>();

            Assert.Same(result1, result2);
        }
        #endregion

        #region Get (named)
        [Fact]
        public void Get_Named_Uninitalized_Throws()
        {
            var sut = Create();

            Assert.Throws<InvalidOperationException>(() => sut.GetEventAggregator<int>(_name));
        }

        [Fact]
        public void Get_Named_With_Invalid_Name_Fails()
        {
            var sut = Create();
            sut.Initialize();

            Assert.Throws<ArgumentException>(() => sut.GetEventAggregator<int>(new Name(null)));
        }

        [Fact]
        public void Get_Named_Creates()
        {
            var sut = Create();
            sut.Initialize();

            sut.GetEventAggregator<int>(_name);

            CallTo(() => _factory.Create<int>()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Get_Named_Returns()
        {
            var sut = Create();
            sut.Initialize();

            var result = sut.GetEventAggregator<int>(_name);

            Assert.Same(_aggregator, result);
        }

        [Fact]
        public void Get_Named_Returns_Same()
        {
            var sut = Create();
            sut.Initialize();

            var result1 = sut.GetEventAggregator<int>(_name);
            var result2 = sut.GetEventAggregator<int>(_name);

            Assert.Same(result1, result2);
        }
        #endregion

        #region Utility
        private EventAggregatorRepository Create() =>
            new EventAggregatorRepository(_factory);
        #endregion
    }
}
