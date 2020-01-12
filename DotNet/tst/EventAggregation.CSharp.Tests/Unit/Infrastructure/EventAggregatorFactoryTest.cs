using SFX.EventAggregation.Infrastructure;
using Xunit;

namespace SFX.EventAggregation.Test.Unit.Infrastructure
{
    [Trait("Category", "Unit")]
    public sealed class EventAggregatorFactoryTest
    {
        #region Type test
        [Fact]
        public void EventAggregatorFactory_Implements_IEventAggregatorFactory() =>
            Assert.True(typeof(IEventAggregatorFactory).IsAssignableFrom(typeof(EventAggregatorFactory)));
        #endregion

        #region Create
        [Fact]
        public void Create_Works()
        {
            var sut = new EventAggregatorFactory();

            var result = sut.Create<int>();

            Assert.NotNull(result);
            Assert.IsType<EventAggregator<int>>(result);
        }
        #endregion
    }
}
