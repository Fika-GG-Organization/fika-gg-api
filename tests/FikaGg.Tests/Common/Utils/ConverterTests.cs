using FikaGg.Api.Common.Utils;
using Xunit.Abstractions;

namespace FikaGg.Tests.Common.Utils;

public class ConverterTests
{
	private readonly ITestOutputHelper _output;

	public ConverterTests(ITestOutputHelper output)
	{
		_output = output;
	}

	[Theory]
	[InlineData("2004-12-13T12:59:41.433Z")]
	[InlineData("2004-12-13 13:59:41.433")]
	[InlineData("2004/12/13T14:59:41.433+2")]
	[InlineData("2004-12-13T00:59:41.433-12:00")]
	[InlineData("12:59:41Z")]
	[InlineData("2004-12-13Z")]
	public void ValidIsoString_Parsing_ReturnUtcDateTime(string isoString)
	{
		// Arrange
		DateTime? test = null;

		// Act
		try
		{
			test = Converter.DateConverter(isoString);
		}
		catch (Exception ex)
		{
			_output.WriteLine(ex.Message);
		}

		// Assert
		var dt = Assert.NotNull(test);
		_output.WriteLine($"Converted DateTime: {dt:O} Kind: {dt.Kind}");
		Assert.Equal(DateTimeKind.Utc, dt.Kind);
	}

	[Theory]
	[InlineData("2004-12-13T08:59:4")]
	[InlineData("04-2-3T8:9:1Z")]
	[InlineData("2004-12-13T29:59:41.433Z")]
	[InlineData("12-13-2004 21:59:41.433Z")]
	[InlineData("20042-12-13 21:59:41.433Z")]
	public void InvalidIsoString_Parsing_ThrowException(string isoString)
	{
		// Arrange
		Exception? exception = null;
		DateTime? test = null;

		// Act
		try
		{
			test = Converter.DateConverter(isoString);
		}
		catch (Exception ex)
		{
			_output.WriteLine(ex.Message);
			exception = ex;
		}

		// Assure
		Assert.Null(test);
		Assert.NotNull(exception);
	}
}
