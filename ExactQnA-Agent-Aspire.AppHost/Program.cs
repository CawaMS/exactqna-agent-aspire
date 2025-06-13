using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddConnectionString("openai");

builder.AddProject<Projects.ChatClient>("ChatClient")
       .WithReference(openai);

builder.Build().Run();
