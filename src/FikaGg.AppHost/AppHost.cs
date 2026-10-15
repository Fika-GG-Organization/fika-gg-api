var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
	.WithDataVolume()
	.WithPgAdmin();

var db = postgres.AddDatabase("fikaggdb");

var rawgApiKey = builder.AddParameter("rawg-api-key", secret: true);

var api = builder.AddProject<Projects.FikaGg_Api>("api")
	.WithReference(db)
	.WaitFor(db)
	.WithEnvironment("Rawg__ApiKey", rawgApiKey);

builder.AddJavaScriptApp("frontend", "../../../fika-gg-web", "dev")
	   .WithReference(api)
	   .WithHttpEndpoint(env: "PORT");

builder.Build().Run();
