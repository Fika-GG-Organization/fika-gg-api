# Middlewares

## Table of Contents

- [Adding IException Middlewares to Pipeline](#adding-iexception-middlewares-to-pipeline)
- [GlobalExceptionsHandler](#globalexceptionshandler)
    - [GetProblemDetailsType](#getproblemdetailstype)
    - [GetStatusCode](#getstatuscode)

## Adding IException Middlewares to Pipeline

Any middlwares that implements `IExceptionHandler` interface must added to `Services` using
The order of these `IExceptionHandler` middlewares determine which one will execute first.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<YOUR OWN CUSTOM HANDLER HERE>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
```

After `app` has been built, add the `UseExceptionHandler()` middleware at the beginning of the 
middlewares order.  

```casharp
var app = builder.Build();

app.UseExceptionHandler();

// The rest of the middlewares...
```

![](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/index/_static/middleware-pipeline.svg?view=aspnetcore-3.1)


## GlobalExceptionsHandler

> Implements -> 
[Microsoft.AspNetCore.Diagnostics.IExceptionHandler](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.diagnostics.iexceptionhandler?view=aspnetcore-10.0)

This is a middleware class that
1. Catches any uncaught exceptions, regardless of their type.
2. Logs the exception message on developer console/terminal.
3. Then translates the exception into a HTTP response (with appropriate status code and message) 
without revealing any internal server details to the client.

### Remarks

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

However, if the `GlobalExceptionHandler` class is added to the application's middleware pipeline, 
those exceptions will be caught and handled automatically.

### Development

`IException` middlewares are purposefully made to handle any uncaught exceptions. 
The `GlobalExceptionHandler` is intended to be the fallback when all else fails, and therefor must 
be added last to `builder.Services`. See 
[How to add IException Middlewares](#how-to-add-iexception-middlewares) for more.

If you want to handle certain types of exceptions in a less generic way, create your own class that
implements the `IExceptionHandler` interface and add it to `builder.Services` above 
`GlobalExceptionHandler`.

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
