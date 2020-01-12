using SFX.CSROP;
using System;

namespace SFX.Utils.Infrastructure
{
    /// <summary>
	/// Interface describing the capability to work with
	/// <see cref="TimeZoneInfo"/>s
	/// </summary>
	public interface ITimeZoneProvider
    {
        /// <summary>
        /// Fetches the local <see cref="TimeZoneInfo"/>
        /// Beware: https://docs.microsoft.com/en-us/dotnet/api/system.timezoneinfo.local?view=netframework-4.8
        /// The local time zone is the time zone on the computer where the code is executing.
        /// </summary>
        TimeZoneInfo GetLocal();

        /// <summary>
        /// Fetches the Utc <see cref="TimeZoneInfo"/>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.timezoneinfo.utc?view=netframework-4.8
        /// </summary>
        TimeZoneInfo GetUtc();

        /// <summary>
        /// Fetches a specific <see cref="TimeZoneInfo"/> based on the provided <paramref name="id"/>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.timezoneinfo.findsystemtimezonebyid?view=netframework-4.8
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Result<TimeZoneInfo> FindSystemTimeZoneById(string id);
    }
}
