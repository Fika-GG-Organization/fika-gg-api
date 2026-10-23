using Microsoft.AspNetCore.Http;
using Xunit.Abstractions;

namespace FikaGg.Tests.Common.Middlewares;

public static class TestOutputHelperExtensions
{
	/// <summary>
	/// Asynchronously writes out <see cref="HttpResponse"/> content for human readability.
	/// </summary>
	/// <param name="output">The <see cref="ITestOutputHelper"/> object that writes to output.</param>
	/// <param name="response">The object to be writted to output</param>
	/// <param name="name">Optional name to help differentiate.</param>
	public static async Task FormatHttpResponseAsync(
		this ITestOutputHelper output,
		HttpResponse response,
		string name = "HttpResponse")
	{
		ArgumentNullException.ThrowIfNull(output);
		ArgumentNullException.ThrowIfNull(response);

		using var reader = new StreamReader(response.Body);

		//reader.BaseStream.Seek(0, SeekOrigin.Begin);

		var body = await reader.ReadToEndAsync();

		var headers = string.Empty;
		foreach (var h in response.Headers)
		{
			headers = $"\n" +
				$"    {h.Key}: {h.Value}";
		}

		output.WriteLine($"{name}\n" +
			$"  StatusCode: {response.StatusCode}\n" +
			$"  Content-Length: {response.ContentLength}\n" +
			$"  Content-Type: {response.ContentType}\n" +
			$"  Headers:{headers}\n" +
			$"  Body: {body}\n");
	}

	/// <summary>
	/// Asynchronously writes out <see cref="ProblemDetailsContext"/> content for human readability.
	/// </summary>
	/// <param name="output">The <see cref="ITestOutputHelper"/> object that writes to output.</param>
	/// <param name="pdContext">The object to be writted to output</param>
	/// <param name="name">Optional name in case of multiple method calls.</param>
	public static async Task FormatProblemDetailsAsync(
		this ITestOutputHelper output,
		ProblemDetailsContext pdContext,
		string name = "ProblemDetails")
	{
		ArgumentNullException.ThrowIfNull(output);
		ArgumentNullException.ThrowIfNull(pdContext);

		var pd = pdContext.ProblemDetails;

		output.WriteLine($"{name}\n" +
			$"  type: \"{pd.Type ?? "null"}\",\n" +
			$"  title: \"{pd.Title ?? "null"}\",\n" +
			$"  status: {pd.Status},\n" +
			$"  detail: \"{pd.Detail ?? "null"}\",\n" +
			$"  instance: {pd.Instance ?? "null"},\n" +
			$"  extensions: [");
	}
}
