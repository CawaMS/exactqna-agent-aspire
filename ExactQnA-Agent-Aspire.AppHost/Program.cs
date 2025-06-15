using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

//Initialize configuration settings for local development
var openai = builder.AddConnectionString("openai");
var aoai_endpoint = builder.AddConnectionString("aoai-endpoint"); // AzureOpenAISettings:Endpoint
var aoai_embedding_deployment = builder.AddConnectionString("aoai-embedding-deployment"); // AzureOpenAISettings:EmbeddingModelDeployment
var aoai_chat_deployment = builder.AddConnectionString("aoai-chat-deployment"); // AzureOpenAISettings:ChatModelDeployment
var aoai_embedding_key = builder.AddConnectionString("aoai-embedding-key"); // AzureOpenAISettings:ApiKeyForEmbeddingDeployment
var aoai_key = builder.AddConnectionString("aoai-key"); // AzureOpenAISettings:ApiKey
var aoai_resource_name = builder.AddConnectionString("aoai-resource-name"); // AzureOpenAISettings:AOAIResourceName
var amr_semantic_cache_provider = builder.AddConnectionString("amr-semantic-cache-provider"); // AzureManagedRedisSettings:SemanticCacheProvider


builder.AddProject<Projects.ChatClient>("ChatClient")
       .WithReference(openai);

builder.AddProject<Projects.AgentAPI>("agentapi")
        .WithReference(aoai_endpoint)
        .WithReference(aoai_embedding_deployment)
        .WithReference(aoai_chat_deployment)
        .WithReference(aoai_embedding_key)
        .WithReference(aoai_key)
        .WithReference(aoai_resource_name)
        .WithReference(amr_semantic_cache_provider);

builder.Build().Run();
