using FikaGg.Api.Common.Middlewares;
using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.Game;
using FikaGg.Api.Integrations.Cloudinary;
using FikaGg.Api.Integrations.Rawg;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<AppDbContext>("fikaggdb");
builder.Services.AddAuthentication(IdentityConstants.BearerScheme);
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins(builder.Configuration["frontend"]??"http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpClient<IRawgClient, RawgClient>(client =>
{
	client.BaseAddress = new Uri("https://api.rawg.io/");
});

builder.Services
	.AddOptions<CloudinarySettings>()
	.Bind(builder.Configuration.GetSection(CloudinarySettings.SectionName))
	.Validate(s => !string.IsNullOrWhiteSpace(s.CloudName)
		&& !string.IsNullOrWhiteSpace(s.ApiKey)
		&& !string.IsNullOrWhiteSpace(s.ApiSecret),
		"Cloudinary settings are missing.")
	.ValidateOnStart();

// Scopes
builder.Services.AddScoped<GameService>();

var app = builder.Build();
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}
else
{
	app.UseExceptionHandler(); // Will catch uncaught exceptions.
	app.UseHsts();             // Tells client browser to switch from HTTP to HTTPS.
}

app.UseHttpsRedirection();
app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapGroup("/api/auth").MapIdentityApi<ApplicationUser>();
app.MapControllers();
await app.RunAsync();
