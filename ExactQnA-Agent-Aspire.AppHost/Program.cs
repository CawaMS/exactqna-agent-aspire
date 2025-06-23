using Aspire.Hosting;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddAzureRedis("redis");

var aoai = builder.AddAzureOpenAI("aoai");

aoai.AddDeployment(
    name: "chatModelDeployment",
    modelName: "gpt-4.1",
    modelVersion: "2025-04-14"
);

aoai.AddDeployment(
    name: "embeddingModelDeployment",
    modelName: "text-embedding-ada-002",
    modelVersion: "2"
);

var agentapi = builder.AddProject<Projects.AgentAPI>("agentapi")
                      .WithExternalHttpEndpoints()
                      .WithReference(aoai)
                      .WithReference(redis)
                      .WaitFor(aoai)
                      .WaitFor(redis);



builder.AddProject<Projects.ChatClient>("chatclient")
       .WithExternalHttpEndpoints()
       .WithReference(agentapi)
       .WithReference(aoai)
       .WaitFor(aoai);


builder.Build().Run();
