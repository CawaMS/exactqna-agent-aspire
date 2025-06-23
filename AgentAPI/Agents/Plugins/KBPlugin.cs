using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.SemanticKernel;
using Redis.OM;
using Redis.OM.Vectorizers;
using StackExchange.Redis;

namespace AgentAPI.Agents.Plugins;

internal sealed class KBPlugin(IConfiguration config, ILogger<KBPlugin> logger)
{
    [KernelFunction]
    public async Task<string> ResponseFromKB(string question)
    {
        logger.LogInformation("Received question: {Question}", question);

        string answer = "";

        ConfigurationOptions options = new ConfigurationOptions
        {
            EndPoints = { config.GetConnectionString("redis") ?? string.Empty }
        };

        // Configure for Azure with DefaultAzureCredential
        await options.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential());

        // Connect to Redis using EntraId authentication
        var muxer = ConnectionMultiplexer.Connect(options);

        // Create Redis OM connection provider using the authenticated connection
        var provider = new RedisConnectionProvider(muxer);

        var cache = provider.AzureOpenAISemanticCache(config.GetConnectionString("aoai-key") ?? string.Empty,
                                                      config.GetConnectionString("aoai-resource-name") ?? string.Empty,
                                                      config.GetConnectionString("embedding-model-id") ?? string.Empty,
                                                      1536, threshold: 0.15);

        // logger.LogInformation("Configuration AOAI-KEY: " + config.GetConnectionString("aoai-key"));

        logger.LogInformation("Initialized Redis connection provider and semantic cache.");

        var result = await cache.GetSimilarAsync(question);

        logger.LogInformation($"Retrieved similar results from the cache.{result.Length}");
        if (result.Length > 0)
        {
            // answer = result[0];

            foreach (var item in result)
            {
                answer += "Answer: " + item + " Score: " + item.Score + "  ";
            }

            logger.LogInformation("Answer found in the knowledge base: {Answer}", answer);

        }
        else
        {
            logger.LogInformation("No answer found in the knowledge base. Nothing returned.");
            // answer = "I don't know about this. Please call the customer service at 000-000-0000 and talk to a representative. Is there anything else I could help you with?";
        }

        return answer;
    }
}
