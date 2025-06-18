using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace AgentAPI.Agents;

public class ExactQnA_Agent
{
    private readonly Kernel _kernel;
    //private readonly IConfiguration _config;
    private readonly ILogger<ExactQnA_Agent> _logger;
    
    public ExactQnA_Agent(Kernel kernel, ILogger<ExactQnA_Agent> logger)
    {
        _kernel = kernel;
        _logger = logger;
    }

    public async Task<string> GetAnswerAsync(string question)
    {
        string result = string.Empty;
        _logger.LogInformation("Received question: {Question}", question);
        _logger.LogInformation("Defining agent...");

        ChatCompletionAgent agent =
            new()
            {
                Name = "ExactAnswerAgent",
                Instructions =
                        """
                        @KBPlugin is a plugin that provides answers to questions based on a knowledge base.
                        You are a question answering agent. You are requested to send the question to the KBPlugin and return the exact answer without rewriting the answer.
                        If KBPlugin does not have an answer, please then create content based on the best of your knowledge.
                        """,
                Kernel = _kernel,
                Arguments =
                    new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
                    {
                        { "question", question }
                    }
            };

        _logger.LogInformation("Agent defined. Starting to generate answers");

        ChatHistoryAgentThread agentThread = new();

        var message = new ChatMessageContent(AuthorRole.User, question);

        KernelArguments arguments =
        new()
        {
        // { "now", $"{now.ToShortDateString()} {now.ToShortTimeString()}" }
        };

        await foreach (ChatMessageContent response in agent.InvokeAsync(message, agentThread, options: new() { KernelArguments = arguments }))
        {
            // Display response.
            _logger.LogInformation($"{response.Content}");
            result += response.Content;
        }

        return result;
    }
}
