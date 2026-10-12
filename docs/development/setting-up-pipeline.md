# Setting Up Middlewares

Middlewares are code that can modify:

- a HTTP request *before* it reaches an endpoint. 
- a HTTP response *after* it leaves an endpoint. 

An application commonly have multiple middlewares in its pipeline, and a middleware can choose 
to pass a request to the next middleware without modifying the HTTP 
request/response.

## Table of Contents

- [1. Middleware Order](#middleware-order)
- [2. List of Middlewares](#list-of-middlewares)
    - [2.1. Exception Handlers](#exception-handlers)
    - [2.2. HSTS](#hsts)
    - [2.3. HTTPS Redirection](#https-redirection)
    - [2.4. Cookie Policy](#cookie-policy)
    - [2.5. Routing](#routing)
    - [2.6. CORS](#cors)
    - [2.7. Authentication](#authentication)
    - [2.8. Authorization](#authorization)
- [3. Build Services](#build-services)
    - [3.1. Adding Dependencies](#adding-dependencies)
    - [3.2. Adding Exception Handlers](#adding-exception-handlers)
    - [3.2. Adding CORS policy](#adding-cors-policy)


## 1. Middleware Order

Setting up the app pipeline's middleware order is very important, as a HTTP request will 
pass through each middleware in the order they were set up. 

This means that the middleware that was added first is always:
- the first middleware a HTTP request have to pass through before reaching the second 
middleware, etc...
- the last middleware a HTTP response have to pass through before exiting the application.

Additionally, some middlewares **MUST** be set up in a specific order or else the app might 
not run correctly.

A typical ASP.NET pipeline might look like this:

![standard asp.net 3.1 middleware pipeline order](images/middleware-pipeline-order.png)

In `fika-gg-api`, 

```csharp
var app = builder.Build();

// Other code...

app.UseExceptionHandler();
app.UseHsts();
app.UseHttpsRedirection();
app.UseCookiePolicy();         // Optional
app.UseRouting();              // Optional
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

For more, see [Middleware order🔗‍️](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0#middleware-order)

## 2. List of Middlewares

### 2.1. Exception Handlers

Place at the start of the pipeline.

```csharp
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}
else {
    app.UseExceptionHandler();
}
```

These middlewares will detect and handle exceptions.

- If environment is `Development`, `UseDeveloperExceptionPage()` should be used.
- If environment is not `Development`, `UseExceptionHandler()` should be used.

See [Adding Exception Handlers](#adding-exception-handlers) for more about 
`UseExceptionHandler()`

### 2.2. HSTS

This middleware should only be set up when the app runs in the `Production` environment. 

```csharp
if (app.Environment.IsProduction()) {
    app.UseHsts();
}
```

Sets all HTTP responses to have the `Strict-Transport-Security` response header.
This tells the client browser to switch from HTTP to HTTPS.

For more, see [Strict-Transport-Security🔗‍️](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Strict-Transport-Security).

### 2.3. HTTPS Redirection

```csharp
app.UseHttpsRedirection();
```

Converts all client HTTP requests to use HTTPS.

### 2.4. Cookie Policy

**Optional** because `AspNetCore.Identity` has built in cookie policy.

```csharp
app.UseCookiePolicy();
```

Sets cookie consent policy such as `SameSite` and `secure`.

Allows cookie policy to conform to **EU General Data Protection Regulation** (GDPR).

### 2.5. Routing

**Optional** because **ASP.NET Core** automatically adds it to the start of the 
middleware pipeline.

```csharp
app.UseRouting();
```

### 2.6. CORS

Must be placed immediately before [`Authentication`](#authentication).

```csharp
app.UseCors();
```

Sets a **Cross-Origin Resource Sharing** (CORS) policy to server. 

This CORS policy must be defined by using `AddCors()`. 
See [Adding CORS Policy](setting-up-builder-services.md).

### 2.7. Authentication

**Required** for `AspNetCore.Identity`.

```csharp
app.UseAuthentication();
```

Provides support for user authentication (e.g. sign in with credentials, user registration, 
check login, etc).

### 2.8. Authorization

Must be placed immediately after [`Authentication`](#authentication).

```csharp
app.UseAuthorization();
```

Provides support for role permissions.

---

## 3. Build Services

### 3.1. Adding Dependencies

[Dependency Injection🔗‍️](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0)

`AddSingleton()`

`AddScoped()`

`AddTransient()`


### 3.2. Adding Exception Handlers

The `UseExceptionHandler()` middleware will catch any exceptions not caught by `try...catch` 
blocks. 
To set it up, we need to:

1. Add `AddProblemDetails()`, in `Program.cs`.

    ```csharp
    builder.Services.AddProblemDetails();
    ```

2. Create a custom class that implements the `IExceptionHandler` interface.
    1. For an example, see [Handle errors in ASP.NET Core🔗‍️](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-10.0#iexceptionhandler).
3. Add the custom class to using `AddExceptionHandler()`, in `Program.cs`.

    ```csharp
    builder.Services.AddProblemDetails();

    builder.Services.AddExceptionHandler<CustomExceptionHandler>();
    builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    ```

4. The order of `AddExceptionHandler()` determine in which order the `IExceptionHandler` 
classes are executed. From the first `IExceptionHandler` added, to the second, to the third, 
etc.
    1. If we have an `IExceptionHandler` class that only checks for specific exceptions in a 
specific way, we should place it earlier.
    2. The custom `GlobalExceptionHandler` class is intended to be the fallback handler when 
all else fails. It must be last in order.

### 3.3. Adding CORS policy

Most browser clients implement a **Same-Origin Policy** for security reasons.
By default, a server can share a resource to a client on the same **origin**, but not 
when a client is from a different **origin**.

- An **origin** is an URL/web address.

To allow **cross-origin requests**, the server must set a CORS policy with a list of 
trusted URLs, as well as allowed HTTP methods.

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "PolicyName", policy =>
    {
        policy
        .WithOrigins("http://example.se")  // Sets which origins can have server resources
        .WithMethods("GET", "POST")        // Sets which HTTP methods origins can use
        .WithHeaders()                     // Sets allowed HTTP headers
        .AllowCredentials()                // Lets origins to send login info to server
    });
});
```

For more, see [Enable Cross-Origin Requests (CORS) in ASP.NET Core🔗‍️](https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-10.0)
