using FikaGg.Api.Common.Enums;
using FikaGg.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FikaGg.Api.Data;

// fills the local db with test users, games and events so theres stuff to click on
// only runs in development, and only when there are no events yet
public static class DevDataSeeder
{
	private static readonly (string Email, string Name)[] SeedUsers =
	[
		("alex@fika.test", "Alex K"),
		("noor@fika.test", "Noor H"),
		("sam@fika.test", "Sam L"),
		("elin@fika.test", "Elin B"),
		("omar@fika.test", "Omar S"),
		("linnea@fika.test", "Linnea P"),
		("jonas@fika.test", "Jonas W"),
		("sara@fika.test", "Sara M"),
		("ali@fika.test", "Ali R"),
		("moa@fika.test", "Moa E"),
	];

	private static readonly string[] GameNames =
	[
		"Counter-Strike 2", "Minecraft", "League of Legends", "Fortnite",
		"Rocket League", "Overwatch 2", "Apex Legends",
	];

	private static readonly string[] Titles =
	[
		"Fredagsmys", "Chill kväll", "Ranked grind", "Nybörjarvänligt",
		"Sena timmar", "Söndagsspel", "Snabb runda efter plugget", "Turnering med polarna",
	];

	private static readonly string?[] Descriptions =
	[
		"voice på discord, alla välkomna",
		"vi kör ranked så helst gold+",
		"helt chill, ingen tryhard",
		null,
	];

	public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
	{
		var db = services.GetRequiredService<AppDbContext>();
		var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

		// already seeded or someone has real test data, dont touch it
		if (await db.Events.AnyAsync())
		{
			return;
		}

		// no password set, skip it so we never have to put one in the code
		var password = config["Seed:Password"];
		if (string.IsNullOrWhiteSpace(password))
		{
			return;
		}

		var users = new List<ApplicationUser>();
		foreach (var (email, name) in SeedUsers)
		{
			users.Add(await CreateUserAsync(userManager, email, name, password));
		}

		// valorant has its real rawg id so picking it in skapa event reuses this row
		var games = new List<Game>
		{
			await db.Games.FirstOrDefaultAsync(g => g.RawgId == 415171)
				?? new Game { RawgId = 415171, Name = "Valorant" },
		};
		games.AddRange(GameNames.Select(name => new Game { Name = name }));

		// same seed = same data every time, so everyone sees the same thing
		var random = new Random(42);
		var now = DateTime.UtcNow;

		for (var i = 0; i < 40; i++)
		{
			var host = users[random.Next(users.Count)];
			var maxPlayers = random.Next(2, 7);

			// from a week ago to 4 weeks ahead, evenings (utc so its an hour later in sweden)
			var start = now.Date.AddDays(random.Next(-7, 29)).AddHours(random.Next(16, 21));
			var createdAt = start.AddDays(-random.Next(1, 7));
			if (createdAt > now)
			{
				createdAt = now;
			}

			// host first, then a random bunch of others. some events get more than max so we get backups
			var others = users
				.Where(u => u != host)
				.OrderBy(_ => random.Next())
				.Take(random.Next(0, maxPlayers + 2))
				.ToList();

			var participants = new List<EventParticipant>
			{
				Player(host, ParticipantStatus.Joined, createdAt),
			};
			for (var p = 0; p < others.Count; p++)
			{
				// the host already takes one spot
				var status = p < maxPlayers - 1 ? ParticipantStatus.Joined : ParticipantStatus.Backup;
				participants.Add(Player(others[p], status, createdAt.AddMinutes(p + 1)));
			}

			db.Events.Add(new Event
			{
				HostId = host.Id,
				Game = games[random.Next(games.Count)],
				Title = Titles[random.Next(Titles.Length)],
				Description = Descriptions[random.Next(Descriptions.Length)],
				StartTime = start,
				EndTime = start.AddHours(random.Next(1, 4)),
				MaxPlayers = maxPlayers,
				CreatedAt = createdAt,
				Participants = participants,
			});
		}

		await db.SaveChangesAsync();
	}

	private static async Task<ApplicationUser> CreateUserAsync(
		UserManager<ApplicationUser> userManager, string email, string displayName, string password)
	{
		// reuse the user if its already there from an earlier run
		var user = await userManager.FindByEmailAsync(email);
		if (user != null)
		{
			return user;
		}

		user = new ApplicationUser { UserName = email, Email = email, DisplayName = displayName, EmailConfirmed = true };
		var result = await userManager.CreateAsync(user, password);
		if (!result.Succeeded)
		{
			throw new InvalidOperationException($"Could not seed {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
		}

		return user;
	}

	private static EventParticipant Player(ApplicationUser user, ParticipantStatus status, DateTime joinedAt) =>
		new() { UserId = user.Id, Status = status, JoinedAt = joinedAt };
}
