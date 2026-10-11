using FikaGg.Api.Common.Utils;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace FikaGg.Api.Common.Middlewares;

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

		var status = StatusCodesUtils.GetStatusCode(exception);

		// Uncomment this, if `Status = status.Key` does override default status code.
		// httpContext.Response.StatusCode = status.Key;

		return await _problemDetailsService.TryWriteAsync(new()
		{
			HttpContext = httpContext,
			Exception = exception,
			ProblemDetails = new ProblemDetails()
			{
				Status = status.Key,
				Type = StatusCodesUtils.GetProblemDetailsType(status.Key),
				Title = ReasonPhrases.GetReasonPhrase(status.Key),
				Detail = status.Value,
			}
		});
	}

}
