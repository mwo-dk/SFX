using SFX.CSharp.ROP;
using System;

namespace SFX.CSharp.Utils.Infrastructure
{
    /// <summary>
	/// Facilitates to convert <see cref="DateTimeOffset"/>s between
	/// <see cref="TimeZoneInfo"/>s
	/// </summary>
	public interface IDateTimeConverter
    {
        /// <summary>
        /// Converts <paramref name="dateTimeOffset"/> to the <see cref="DateTimeOffset"/>
        /// as if seen by the provided <see cref="TimeZoneInfo"/> provided
        /// (<paramref name="timeZoneInfo"/>)
        /// </summary>
        /// <param name="dateTimeOffset">The source <see cref="DateTimeOffset"/></param>
        /// <param name="timeZoneInfo">The destination <see cref="TimeZoneInfo"/></param>
        /// <returns></returns>
        Result<DateTimeOffset> Convert(DateTimeOffset dateTimeOffset, TimeZoneInfo timeZoneInfo);

        /// <summary>
        /// Converts <paramref name="dateTimeOffset"/> to Utc
        /// </summary>
        /// <param name="dateTimeOffset">The source <see cref="DateTimeOffset"/></param>
        /// <returns></returns>
        Result<DateTimeOffset> ToUtc(DateTimeOffset dateTimeOffset);
    }
}
