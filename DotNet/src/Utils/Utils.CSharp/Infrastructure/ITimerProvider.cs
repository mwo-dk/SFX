using SFX.CSharp.ROP;
using System;

namespace SFX.CSharp.Utils.Infrastructure
{
    /// <summary>
    /// Interface describing the capability to create <see cref="ITimer"/>s
    /// </summary>
    public interface ITimerProvider
    {
        /// <summary>
        /// Creates a new <see cref="ITimer"/>
        /// </summary>
        /// <param name="interval">The <see cref="TimeSpan"/> describing the interval of repetition</param>
        /// <param name="handler">The handler</param>
        /// <param name="autoStart">Flag telling whether to automatically start the timer</param>
        /// <returns>The newly created <see cref="ITimer"/></returns>
        Result<ITimer> Create(TimeSpan interval, Action handler, bool autoStart);
    }
}
