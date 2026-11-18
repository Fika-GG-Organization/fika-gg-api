using System.Globalization;

namespace FikaGg.Api.Common.Utils;

public static class Converter
{
	/// <summary>
	/// Validates a string that represents date-time, and converts it into the equivalent UTC
	/// <see cref="DateTime"/>.<br/><br/>
	/// </summary>
	/// <remarks>
	/// JSON does not have a native date-time data type, instead the standard across the internet <br/>
	/// (including HTTP) is to serialize dates as strings using the <a href="https://iso8601.com/">ISO 8601</a> format.<br/><br/>
	/// 
	/// This helper method ensures that converted DateTime objects has their <see cref="DateTimeKind"/> set<br/>
	/// to Utc to prevent any time zone discrepancy while saving to the database.
	/// </remarks>
	/// <param name="dateString">
	/// An string that represents date and time. Example: <c>2012-01-30T23:59:01.321Z</c>.
	/// </param>
	/// <returns>A <see cref="DateTime"/> object with the UTC/GMT time zone.</returns>
	/// <exception cref="FormatException"></exception>
	public static DateTime DateConverter(string dateString)
	{
		if (!DateTime.TryParse(dateString, null, DateTimeStyles.RoundtripKind, out var parsedDate))
		{
			throw new FormatException("Invalid date format. Please provide a valid date.");
		}

		return parsedDate.ToUniversalTime();
	}
}
