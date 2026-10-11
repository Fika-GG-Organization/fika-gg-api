using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace FikaGg.Api.Common.Utils;

public static class StatusCodesUtils
{
	private static readonly Dictionary<int, string> _problemDetailsTypes = new Dictionary<int, string>()
	{
		{ 400, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.1" },
		{ 401, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.2" },
		{ 402, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.3" },
		{ 403, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.4" },
		{ 404, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.5" },
		{ 405, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.6" },
		{ 406, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.7" },
		{ 407, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.8" },
		{ 408, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.9" },
		{ 409, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.10" },
		{ 410, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.11" },
		{ 411, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.12" },
		{ 412, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.13" },
		{ 413, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.14" },
		{ 414, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.15" },
		{ 415, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.16" },
		{ 416, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.17" },
		{ 417, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.18" },
		{ 418, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.19" },
		{ 421, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.20" },
		{ 422, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.21" },
		{ 426, "https://www.rfc-editor.org/info/rfc9110/#section-15.5.22" },
		{ 500, "https://www.rfc-editor.org/info/rfc9110/#section-15.6.1" },
		{ 501, "https://www.rfc-editor.org/info/rfc9110/#section-15.6.2" },
		{ 502, "https://www.rfc-editor.org/info/rfc9110/#section-15.6.3" },
		{ 503, "https://www.rfc-editor.org/info/rfc9110/#section-15.6.4" },
		{ 504, "https://www.rfc-editor.org/info/rfc9110/#section-15.6.5" },
		{ 505, "https://www.rfc-editor.org/info/rfc9110/#section-15.6.6" },
	};

	public static string GetProblemDetailsType(int statusCode)
	{
		return _problemDetailsTypes.GetValueOrDefault(statusCode, "Unknown Type");
	}

	public static KeyValuePair<int, string> GetStatusCode(Exception exception)
	{
		return exception switch
		{
			DbUpdateException => new(StatusCodes.Status409Conflict, "Failed to save to database."),
			TimeoutException => new(StatusCodes.Status408RequestTimeout, "Request could not completed in time."),
			ArgumentNullException => new(StatusCodes.Status404NotFound, "Requested resource could not be found."),
			IndexOutOfRangeException => new(StatusCodes.Status404NotFound, "Requested resource could not be found."),
			KeyNotFoundException => new(StatusCodes.Status404NotFound, "Requested resource could not be found."),
			ArgumentException => new(StatusCodes.Status400BadRequest, "Try again."),
			ApplicationException => new(StatusCodes.Status400BadRequest, "Try again."),
			ArithmeticException => new(StatusCodes.Status400BadRequest, "Try again."),
			AuthenticationException => new(StatusCodes.Status400BadRequest, "Try again."),
			FormatException => new(StatusCodes.Status400BadRequest, "Invalid formatting. Try again."),
			InvalidCastException => new(StatusCodes.Status400BadRequest, "Try again."),
			InvalidOperationException => new(StatusCodes.Status400BadRequest, "Try again."),
			InvalidTimeZoneException => new(StatusCodes.Status400BadRequest, "Invalid timezone. Try again."),
			AuthenticationFailureException => new(StatusCodes.Status400BadRequest, "Try again."),
			NotImplementedException => new(StatusCodes.Status501NotImplemented, "Service is not implemented."),
			NotSupportedException => new(StatusCodes.Status501NotImplemented, "Service is not implemented."),
			DllNotFoundException => new(StatusCodes.Status500InternalServerError, "Oopsie daisy, we did a fuwky wucky."),
			IOException => new(StatusCodes.Status500InternalServerError, "Oopsie daisy, we did a fuwky wucky."),
			SystemException => new(StatusCodes.Status500InternalServerError, "Oopsie daisy, we did a fuwky wucky."),
			_ => new(StatusCodes.Status500InternalServerError, "Oopsie daisy, we did a fuwky wucky."),
		};
	}

}
