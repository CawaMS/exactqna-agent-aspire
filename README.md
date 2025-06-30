# Exact QnA Agent using .NET Aspire, Semantic Kernel SDK, and Azure Managed Redis

## Overview
The Exact QnA Agent returns consistency and accurate answer from the various questions customers ask. It is built using the ChatCompletionAgent using Semantic Kernel SDK and the .NET Aspire framework. Azure Managed Redis is used as a knowledge store for providing consistent answers to the questions. It's accomplished through vector similarity search with the Semantic Kernel plugin pattern.

### Example output of the agent application

![Example output screenshot](./media/example-output.png)

## Run in Azure

### Deployment Steps

1. Download or fork / clone this project
1. Open a command prompt
1. Change directory to the AppHost folder
1. run ```azd up```
1. Change directory to *Preload-KB-Redis* folder. Set the following user-secrets:
    - Redis:SemanticCacheAzureProvider <rediss://:redis-primary-key@redis-domain-name:10000>
    - Redis:connectionString <redis-domain-name:10000,password=redis-primary-key,ssl=True,abortConnect=False>
    - AOAIResourceName <i.e. myAOAIResource, just the name, without domain>
    - AOAI:endpoint <https://resource-name.openai.azure.com/>
    - AOAI:embeddingDeploymentName <i.e. myEmbeddingDeploymentName>
    - AOAI:apiKey <i.e. AOAI API Key>

### Resources provisioned
- Azure Container Environment
- Two Container apps: *ChatClient* and *AgentAPI*
- Azure Managed Redis
- Azure Open AI
- GPT 4.1 model
- Ada Text Embedding model

### Clean up

run ```azd down```