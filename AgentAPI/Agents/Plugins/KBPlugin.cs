using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.SemanticKernel;
using Redis.OM;
using Redis.OM.Vectorizers;

namespace AgentAPI.Agents.Plugins;

internal sealed class KBPlugin(IConfiguration config)
{
    [KernelFunction]
    public string ResponseFromKB(string question)
    {
        string answer = "";
        var provider = new RedisConnectionProvider(config.GetConnectionString("amr-semantic-cache-provider") ?? string.Empty);
        var cache = provider.AzureOpenAISemanticCache(config.GetConnectionString("aoai-embedding-key") ?? string.Empty,
                                                      config.GetConnectionString("aoai-resource-name") ?? string.Empty,
                                                      config.GetConnectionString("aoai-embedding-deployment") ?? string.Empty,
                                                      1536, threshold: 0.30);

        var result = cache.GetSimilar(question);
        if (result.Length > 0)
        {
            answer = result[0];
        }
        else
        {
            answer = "I don't know about this. Please call the customer service at 000-000-0000 and talk to a representative. Is there anything else I could help you with?";
        }

        return answer;
    }
}
