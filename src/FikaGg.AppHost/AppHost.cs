var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var db = postgres.AddDatabase("fikaggdb");

var api = builder.AddProject<Projects.FikaGg_Api>("api")
    .WithReference(db)
    .WaitFor(db);

builder.AddJavaScriptApp("frontend", "../../../fika-gg-web", "dev")
       .WithReference(api)
       .WithHttpEndpoint(port: 3000, env: "PORT");

builder.Build().Run();