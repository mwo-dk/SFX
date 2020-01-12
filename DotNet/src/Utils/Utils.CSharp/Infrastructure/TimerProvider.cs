using SFX.CSharp.ROP;
using System;
using static SFX.CSharp.ROP.Library;

namespace SFX.CSharp.Utils.Infrastructure
{
    /// <summary>
    /// Implements <see cref="ITimerProvider"/>
    /// </summary>
    public sealed class TimerProvider : ITimerProvider
    {
        /// <inheritdoc/>
        public Result<ITimer> Create(TimeSpan interval, Action handler, bool autoStart)
        {
            try
            {
                var result = new Timer(interval, handler);
                if (autoStart)
                    result.Start();
                return Succeed(result as ITimer);
            }
            catch (Exception error)
            {
                return Fail<ITimer>(error);
            }
        }
    }
}
