using SFX.CSharp.ROP;
using System;

namespace SFX.CSharp.Utils.Infrastructure
{
    /// <summary>
    /// Represents a timer
    /// </summary>
    public interface ITimer : IDisposable
    {
        /// <summary>
        /// Starts the timer
        /// </summary>
        Result<Unit> Start();
        /// <summary>
        /// Stops the timer
        /// </summary>
        Result<Unit> Stop();
    }
}