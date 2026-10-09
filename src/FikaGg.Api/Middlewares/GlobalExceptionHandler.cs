using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace FikaGg.Api.Middlewares
{
	public sealed partial class GlobalExceptionHandler : IExceptionHandler
	{
		private readonly ILogger<GlobalExceptionHandler> _logger;
		private readonly IProblemDetailsService _problemDetailsService;

		public GlobalExceptionHandler(
			ILogger<GlobalExceptionHandler> logger,
			IProblemDetailsService problemDetailsService)
		{
			ArgumentNullException.ThrowIfNull(logger);
			ArgumentNullException.ThrowIfNull(problemDetailsService);

			_logger = logger;
			_problemDetailsService = problemDetailsService;
		}


		[LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "An exception has occured: ", EventName = nameof(LogException))]
		private static partial void LogException(ILogger logger, Exception exception);


		public async ValueTask<bool> TryHandleAsync(
			HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
		{
			// Prints out exception message to developer terminal without revealing it to client.
			LogException(_logger, exception);

			var status = GetStatusCode(exception);

			// Uncomment this, if `Status = status.Key` does override default status code.
			// httpContext.Response.StatusCode = status.Key;

			return await _problemDetailsService.TryWriteAsync(new()
			{
				HttpContext = httpContext,
				Exception = exception,
				ProblemDetails = new ProblemDetails()
				{
					Status = status.Key,
					Type = GetProblemDetailsType(status.Key),
					Title = ReasonPhrases.GetReasonPhrase(status.Key),
					Detail = status.Value,
				}
			});
		}


		private static KeyValuePair<int, string> GetStatusCode(Exception exception)
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


		private static string GetProblemDetailsType(int statusCode)
		{
			return statusCode switch
			{
				StatusCodes.Status400BadRequest => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.1",
				StatusCodes.Status401Unauthorized => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.2",
				StatusCodes.Status402PaymentRequired => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.3",
				StatusCodes.Status403Forbidden => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.4",
				StatusCodes.Status404NotFound => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.5",
				StatusCodes.Status405MethodNotAllowed => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.6",
				StatusCodes.Status406NotAcceptable => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.7",
				StatusCodes.Status407ProxyAuthenticationRequired => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.8",
				StatusCodes.Status408RequestTimeout => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.9",
				StatusCodes.Status409Conflict => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.10",
				StatusCodes.Status410Gone => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.11",
				StatusCodes.Status411LengthRequired => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.12",
				StatusCodes.Status412PreconditionFailed => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.13",
				StatusCodes.Status413PayloadTooLarge => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.14",
				StatusCodes.Status414UriTooLong => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.15",
				StatusCodes.Status415UnsupportedMediaType => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.16",
				StatusCodes.Status416RangeNotSatisfiable => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.17",
				StatusCodes.Status417ExpectationFailed => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.18",
				StatusCodes.Status418ImATeapot => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.19",
				StatusCodes.Status421MisdirectedRequest => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.20",
				StatusCodes.Status422UnprocessableEntity => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.21",
				StatusCodes.Status426UpgradeRequired => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.22",
				StatusCodes.Status500InternalServerError => "https://www.rfc-editor.org/info/rfc9110/#section-15.6.1",
				StatusCodes.Status501NotImplemented => "https://www.rfc-editor.org/info/rfc9110/#section-15.6.2",
				StatusCodes.Status502BadGateway => "https://www.rfc-editor.org/info/rfc9110/#section-15.6.3",
				StatusCodes.Status503ServiceUnavailable => "https://www.rfc-editor.org/info/rfc9110/#section-15.6.4",
				StatusCodes.Status504GatewayTimeout => "https://www.rfc-editor.org/info/rfc9110/#section-15.6.5",
				StatusCodes.Status505HttpVersionNotsupported => "https://www.rfc-editor.org/info/rfc9110/#section-15.6.6",
				_ => "Unknown Type",
			};
		}

	}
}
