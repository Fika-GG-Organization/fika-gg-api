var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.FikaGg_Api>("api");

builder.AddJavaScriptApp("frontend", "../../../fika-gg-frontend", "dev")
       .WithReference(api)
       .WithHttpEndpoint(port: 3000, env: "PORT");

builder.Build().Run();