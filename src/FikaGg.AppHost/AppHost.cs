var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
	.WithDataVolume()
	.WithPgAdmin();

var db = postgres.AddDatabase("fikaggdb");

var rawgApiKey = builder.AddParameter("rawg-api-key", secret: true);

var cloudinaryCloudName = builder.AddParameter("cloudinary-cloud-name");
var cloudinaryApiKey = builder.AddParameter("cloudinary-api-key", secret: true);
var cloudinaryApiSecret = builder.AddParameter("cloudinary-api-secret", secret: true);

var api = builder.AddProject<Projects.FikaGg_Api>("api")
	.WithReference(db)
	.WaitFor(db)
	.WithEnvironment("Rawg__ApiKey", rawgApiKey)
	.WithEnvironment("Cloudinary__CloudName", cloudinaryCloudName)
	.WithEnvironment("Cloudinary__ApiKey", cloudinaryApiKey)
	.WithEnvironment("Cloudinary__ApiSecret", cloudinaryApiSecret);

builder.AddJavaScriptApp("frontend", "../../../fika-gg-web", "dev")
	   .WithReference(api)
	   .WithHttpEndpoint(env: "PORT");

builder.Build().Run();
