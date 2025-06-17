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
                        You are an agent designed to answer question only from the KBPlugin plugin. Do not generate any other content if the KBPlugin could find an answer.
                        The {{$question}} is what user asks as input message. Just run the user input through by the KBPlugin plugin.
                        If the KBPlugin plugin cannot find an answer, then you can generate a response to the user.
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
