using SFX.CSharp.ROP;
using System;
using static SFX.CSharp.ROP.Library;

namespace SFX.CSharp.Utils.Infrastructure
{
    /// <summary>
	/// Implements <see cref="IDateTimeConverter"/>
	/// </summary>
	public sealed class DateTimeConverter : IDateTimeConverter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timeZoneProvider">The utilized <see cref="ITimeZoneProvider"/></param>
        public DateTimeConverter(ITimeZoneProvider timeZoneProvider) =>
            TimeZoneProvider = timeZoneProvider ?? throw new ArgumentNullException(nameof(timeZoneProvider));

        internal ITimeZoneProvider TimeZoneProvider { get; }

        /// <inheritdoc/>
        public Result<DateTimeOffset> Convert(DateTimeOffset dateTimeOffset, TimeZoneInfo timeZoneInfo)
        {
            try
            {

                return Succeed(TimeZoneInfo.ConvertTime(dateTimeOffset, timeZoneInfo));
            }
            catch (Exception error)
            {
                return Fail<DateTimeOffset>(error);
            }
        }

        /// <inheritdoc/>
        public Result<DateTimeOffset> ToUtc(DateTimeOffset dateTimeOffset)
        {
            try
            {
                return Succeed(TimeZoneInfo.ConvertTime(dateTimeOffset, TimeZoneProvider.GetUtc()));
            }
            catch (Exception error)
            {
                return Fail<DateTimeOffset>(error);
            }
        }
    }
}
