using System;
using System.Threading;
using System.Threading.Tasks;
using FikaGg.Api.Common.Middlewares;
using FikaGg.Api.Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Xunit;
using Xunit.Abstractions;

namespace FikaGg.Tests;

public class GlobalExceptionHandlerTests
{
	/// <summary>
	/// <see cref="ITestOutputHelper"/> should be used to write to Test Detail Summary output.<br /><br />
	/// <see cref="Console.WriteLine()"/> does not work in xUnit test methods.
	/// </summary>
	private readonly ITestOutputHelper _output;

	public GlobalExceptionHandlerTests(ITestOutputHelper output)
	{
		_output = output;
	}


	[Fact]
	public async Task TryHandleAsync_ValidException_LogsAndWritesProblemDetails()
	{
		// Arrange
		var httpContext = new DefaultHttpContext();
		var exception = new InvalidOperationException("Something went wrong");
		var cancellationToken = CancellationToken.None;

		var fakeLogger = new FakeLogger<GlobalExceptionHandler>();
		var fakeProblemDetailsService = new FakeProblemDetailsService(shouldSucceed: true);

		var handler = new GlobalExceptionHandler(fakeLogger, fakeProblemDetailsService);

		//Act
		var result = await handler.TryHandleAsync(httpContext, exception, cancellationToken);

		//Assert
		Assert.True(result, $"Assert failed: {nameof(result)} was not true.");

	
		var logEntry = Assert.Single(fakeLogger.LoggedEntries);
		Assert.Equal(LogLevel.Warning, logEntry.LogLevel);
		Assert.Equal(exception, logEntry.Exception);

		_output.WriteLine(logEntry.ToString());

		// Verify ProblemDetailsService was invoked with correct data
		Assert.NotNull(fakeProblemDetailsService.LastContext);
		Assert.Equal(httpContext, fakeProblemDetailsService.LastContext.HttpContext);
		Assert.Equal(exception, fakeProblemDetailsService.LastContext.Exception);
		Assert.NotNull(fakeProblemDetailsService.LastContext.ProblemDetails);
	}

	[Fact]
	public async Task TryHandleAsync_WhenProblemDetailsServiceFails_ReturnsFalse()
	{
		// Arrange
		var httpContext = new DefaultHttpContext();
		var exception = new ArgumentNullException("param");
		var cancellationToken = CancellationToken.None;

		var fakeLogger = new FakeLogger<GlobalExceptionHandler>();
		var fakeProblemDetailsService = new FakeProblemDetailsService(shouldSucceed: false);

		var handler = new GlobalExceptionHandler(fakeLogger, fakeProblemDetailsService);

		// Act
		var result = await handler.TryHandleAsync(httpContext, exception, cancellationToken);

		// Assert
		Assert.False(result);
	}



	private sealed class FakeLogger<T> : ILogger<T>
	{
		public List<(LogLevel LogLevel, EventId EventId, Exception? Exception, string Message)> LoggedEntries { get; } = new();

		IDisposable? ILogger.BeginScope<TState>(TState state) => null;

		bool ILogger.IsEnabled(LogLevel logLevel) => true;

		void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			LoggedEntries.Add((logLevel, eventId, exception, formatter(state, exception)));
		}
	}

	
	private sealed class FakeProblemDetailsService : IProblemDetailsService
	{
		private readonly bool _shouldSucceed;
		public ProblemDetailsContext? LastContext { get; private set; }

		public FakeProblemDetailsService(bool shouldSucceed)
		{
			_shouldSucceed = shouldSucceed;
		}

		public ValueTask<bool> TryWriteAsync(ProblemDetailsContext context)
		{
			LastContext = context;
			return ValueTask.FromResult(_shouldSucceed);
		}

		public ValueTask WriteAsync(ProblemDetailsContext context)
		{
		
			LastContext = context;
			return new ValueTask();
		}
	}
}
