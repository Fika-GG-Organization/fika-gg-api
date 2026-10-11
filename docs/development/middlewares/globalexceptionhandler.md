# GlobalExceptionsHandler

> Implements -> 
[Microsoft.AspNetCore.Diagnostics.IExceptionHandler🔗‍️](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.diagnostics.iexceptionhandler?view=aspnetcore-10.0)

This is middleware class that:
1. Catches any uncaught exceptions, regardless of their type.
2. Logs the exception message on developer console/terminal.
3. Then translates the exception into a HTTP response (with appropriate status code and message) 
without revealing any internal server details to the client.

## Table of Contents

- [Remarks](#remarks)
- [Methods](#methods)
    - [GetProblemDetailsType](#getproblemdetailstype)
    - [GetStatusCode](#getstatuscode)

## Remarks

```csharp
try {
    var result = MethodThatThrowsException();
    return Ok(result);
}
catch(ArgumentException ex) {
    return BadRequest(ex);
}
```

In the code example above, the `try...catch` block will only catch `ArgumentException`. If a 
different `Exception` is thrown inside `MethodThatThrowsException()`, then the server application 
will still crash!

However, if the `GlobalExceptionHandler` class is added to application middleware pipeline, 
those exceptions will be caught and handled automatically.

### Development

This class implements `IExceptionHandler` interface.

Classes that implements `IExceptionHandler` are designed to catch uncaught exceptions. 
The `GlobalExceptionHandler` is intended to be the fallback when all else fails, and therefor must 
be added last to `builder.Services`.

If you want to handle certain types of exceptions in a more specific way, create your own 
class that implements the `IExceptionHandler` interface and add it to `builder.Services` 
above `GlobalExceptionHandler`.

## Methods

`GlobalExceptionHandler` contain the following methods.

### GetProblemDetailsType

This method matches the `4xx`/`5xx` status code and returns link to its 
[RFC 9110](https://www.rfc-editor.org/info/rfc9110) documentation.

This method is only used to populate `ProblemDetails.Type` with content.

#### Development

No additional development needed. This method already checks for every `4xx`/`5xx` status code 
described in RFC 9110.

### GetStatusCode

This method matches the `Exception` type of the parameter and returns the appropriate status code and 
message. Both will be used to write the HTTP response.

The return type is a `KeyValuePair<int, string>` containing the status code (represented by `int`) 
and error message (represented by the `string`).

#### Development

Want to add a status code to another `Exception` class? Feel free to do so! 

Here are some tips.

Be aware that the switch case will show an "unreachable code" error if a child/derived class is 
above its base/parent class. For example, `ArgumentNullException` inherits from `ArgumentException`, 
which means `ArgumentNullException` can not be below `ArgumentException`.

When editing `GetStatusCode` method, avoid using the following `4xx`/`5xx` status codes as they are 
unused, obsolete, or need to be handled in a way that is too complicated for `GlobalExceptionsHandler`. 
See [RFC 9110](https://www.rfc-editor.org/info/rfc9110/#section-15.5) for more.

- `401 Unauthorized`
- `402 Payment Required`
- `405 Method Not Allowed`
- `407 Proxy Authentication Required`
- `418 (Unused)` (Formerly known as `418 Im A Teapot`)
- `419`
- `420`
- `423`
- `424`
- `425`
- `426 Upgrade Required`
