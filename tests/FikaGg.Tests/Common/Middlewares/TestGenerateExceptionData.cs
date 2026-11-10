namespace FikaGg.Tests.Common.Middlewares;

/// <summary>
/// Adds a number of exceptions that can be tested inside a <see cref="ClassDataAttribute"/>.
/// <code>
/// [<see cref="TheoryAttribute"/>]<br />
/// [<see cref="ClassDataAttribute"/>(<see langword="typeof"/>(<see cref="TestGenerateExceptionData"/>))]<br />
/// <see langword="public"/> <see langword="void"/> TestMethod(<see cref="Exception"/> ex)
/// </code>
/// </summary>
public class TestGenerateExceptionData : TheoryData<Exception>
{
	public TestGenerateExceptionData()
	{
		AddRange(
			new ApplicationException(),
			new ArgumentException(),
			new ArgumentNullException(),
			new ArgumentOutOfRangeException(),
			new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException()
				?? new Exception(),
			new Microsoft.EntityFrameworkCore.DbUpdateException()
				?? new Exception(),
			new Exception(),
			new FormatException(),
			new InvalidCastException(),
			new InvalidOperationException(),
			new InvalidTimeZoneException(),
			new SystemException(),
			new System.ComponentModel.DataAnnotations.ValidationException()
				?? new Exception()
			);
	}

	///// <summary>
	///// A list of exception objects to test.<br/><br/>
	///// Every <see cref="object"/>[] represents how many times the method will be tested.<br/><br/>
	///// Every <see langword="new"/> instance inside { } maps to a method parameter.
	///// </summary>
	//private readonly List<object[]> _exceptions = new List<object[]>()
	//{
	//	new object[] { new ApplicationException() },
	//	new object[] { new ArgumentException() },
	//	new object[] { new ArgumentNullException() },
	//	new object[] { new ArgumentOutOfRangeException() },
	//	new object[]
	//	{
	//		new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException()
	//		?? new Exception()
	//	},
	//	new object[]
	//	{
	//		new Microsoft.EntityFrameworkCore.DbUpdateException()
	//		?? new Exception()
	//	},
	//	new object[] { new Exception() },
	//	new object[] { new FormatException() },
	//	new object[] { new InvalidOperationException() },
	//	new object[] { new InvalidCastException() },
	//	new object[] { new InvalidTimeZoneException() },
	//	new object[] { new SystemException() },
	//	new object[]
	//	{
	//		new System.ComponentModel.DataAnnotations.ValidationException()
	//		?? new Exception()
	//	},
	//};

	//public IEnumerator<object[]> GetEnumerator() => _exceptions.GetEnumerator();

	//IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
