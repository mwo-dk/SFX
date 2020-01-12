﻿using System;

namespace SFX.CSharp.Utils.Infrastructure
{
    /// <summary>
    /// Implements <see cref="IDateTimeProvider"/>
    /// </summary>
    public sealed class DateTimeProvider : IDateTimeProvider
    {
        /// <inheritdoc/>
        public DateTimeOffset GetNow() => DateTimeOffset.Now;

        /// <inheritdoc/>
        public DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow;
    }
}
