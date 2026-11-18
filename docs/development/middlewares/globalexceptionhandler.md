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
