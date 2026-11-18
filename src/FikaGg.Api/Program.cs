using FikaGg.Api.Common.Middlewares;
using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.Game;
using FikaGg.Api.Features.User;
using FikaGg.Api.Integrations.CloudinaryConfig;
using FikaGg.Api.Integrations.Rawg;
using FikaGg.Api.Features.EventHandlers;
using FikaGg.Api.Features.EventViews;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using FikaGg.Api.Features.Avatars;
using Microsoft.AspNetCore.DataProtection;
using FikaGg.Api.Features.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<AppDbContext>("fikaggdb");
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

	if (builder.Environment.IsDevelopment())
	{
		// Local development
		options.Cookie.SameSite = SameSiteMode.Lax;
	}
	else
	{
		// Production
		options.Cookie.SameSite = SameSiteMode.None;
	}
});

builder.Services.AddDataProtection().PersistKeysToDbContext<AppDbContext>();

builder.Services.AddCors(options =>
{
	options.AddPolicy("frontend", policy =>
	{
		if (builder.Environment.IsDevelopment())
		{
			policy.WithOrigins("http://localhost:5173", "http://localhost:3000");
		}
		else
		{
			policy.WithOrigins(builder.Configuration["frontend"]!);
		}

		policy.AllowAnyHeader()
		.AllowAnyMethod()
		.AllowCredentials();
	});
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpClient<IRawgClient, RawgClient>(client =>
{
	client.BaseAddress = new Uri("https://api.rawg.io/");
});

builder.Services.AddCloudinary(builder.Configuration);

// Scopes
builder.Services.AddScoped<AvatarService>();
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IEventHandlerService, EventHandlerService>();
builder.Services.AddScoped<IEventViewsService, EventViewsService>();

// Build app
var app = builder.Build();
app.MapDefaultEndpoints();
app.UseExceptionHandler(); // Will catch uncaught exceptions.

// Configure the HTTP request pipeline.

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

	if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("Seed:Reset"))
	{
		// wipes your local db, only when the command is called
		await db.Database.EnsureDeletedAsync();
	}

	await db.Database.MigrateAsync();

	if (app.Environment.IsDevelopment())
	{
		await DevDataSeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
	}
}

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();

	using var scope = app.Services.CreateScope();
}
else
{
	app.UseHsts(); // Tells client browser to switch from HTTP to HTTPS.
}

app.UseHttpsRedirection();
app.UseCors("frontend");

// Prevents using Identity's built in register endpoint
app.Use(async (context, next) =>
{
	if (context.Request.Path.StartsWithSegments("/api/auth/register", StringComparison.OrdinalIgnoreCase))
	{
		context.Response.StatusCode = StatusCodes.Status403Forbidden;
		return;
	}
	await next();
});

app.UseAuthentication();
app.UseAuthorization();
app.MapGroup("/api/auth").MapIdentityApi<ApplicationUser>();
app.MapControllers();
await app.RunAsync();
