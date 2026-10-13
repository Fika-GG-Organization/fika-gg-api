using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FikaGg.Api.Common.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add custom identity endpoints.
/// </summary>
public static class IdentityApiEndpointRouteBuilderExtensions
{
	/// <summary>
	/// Adds an endpoint for logging out because <see cref="Microsoft.AspNetCore.Routing.IdentityApiEndpointRouteBuilderExtensions.MapIdentityApi{TUser}(IEndpointRouteBuilder)"/>
	/// doesn't actually add a log out endpoint?
	/// Despite claiming it does in the XML comment???
	/// </summary>
	/// <typeparam name="TUser"></typeparam>
	/// <param name="endpoints">
	/// The <see cref="IEndpointRouteBuilder"/> to add the logout endpoints to.
	/// Call <see cref="EndpointRouteBuilderExtensions.MapGroup(IEndpointRouteBuilder, string)"/> to add a prefix to all the endpoints.
	/// </param>
	/// <returns>
	/// An <see cref="IEndpointRouteBuilder"/> so that it can be method chained with
	/// <see cref="Microsoft.AspNetCore.Routing.IdentityApiEndpointRouteBuilderExtensions.MapIdentityApi{TUser}(IEndpointRouteBuilder)"/>.
	/// </returns>
	public static IEndpointRouteBuilder MapLogoutIdentityApi<TUser>(
		this IEndpointRouteBuilder endpoints) where TUser : class, new()
	{
		ArgumentNullException.ThrowIfNull(endpoints);

		var routeGroup = endpoints.MapGroup(string.Empty);

		routeGroup.MapGet("/logout", async ([FromServices] IServiceProvider sp) => {
			var signInManager = sp.GetRequiredService<SignInManager<TUser>>();
			await signInManager.SignOutAsync();
		});

		// routeGroup.MapGet("", async() => {
		//
		// });

		return routeGroup;
	}
}
