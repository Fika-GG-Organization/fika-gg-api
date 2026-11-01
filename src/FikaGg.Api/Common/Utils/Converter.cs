namespace FikaGg.Api.Common.Utils;

public static class Converter
{
	public static DateTime DateConverter(string dateString)
	{
		var dt = DateTime.TryParse(dateString, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime date);

		return dt ? date : throw new ArgumentException("Invalid date format. Please provide a valid date.");
	}
}
