# AI Chat with Custom Data

This project is an AI chat application that demonstrates how to chat with custom data using an AI language model. Please note that this template is currently in an early preview stage. If you have feedback, please take a [brief survey](https://aka.ms/dotnet-chat-template-survey).

>[!NOTE]
> Before running this project you need to configure the API keys or endpoints for the providers you have chosen. See below for details specific to your choices.

# Configure the AI Model Provider
## Using Azure OpenAI

To use Azure OpenAI, you will need an Azure account and an Azure OpenAI Service resource. For detailed instructions, see the [Azure OpenAI Service documentation](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource).

### 1. Create an Azure OpenAI Service Resource
[Create an Azure OpenAI Service resource](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource?pivots=web-portal).

### 2. Deploy the Models
Deploy the `gpt-4o-mini` and `text-embedding-3-small` models to your Azure OpenAI Service resource. When creating those deployments, give them the same names as the models (`gpt-4o-mini` and `text-embedding-3-small`). See the Azure OpenAI documentation to learn how to [Deploy a model](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource?pivots=web-portal#deploy-a-model).

### 3. Configure Azure OpenAI for Keyless Authentication
This template is configured to use keyless authentication (also known as Managed Identity, with Entra ID). In the Azure Portal, when viewing the Azure OpenAI resource you just created, view access control settings and assign yourself the `Cognitive Services OpenAI User` role. [Learn more](https://learn.microsoft.com/azure/developer/ai/keyless-connections).

### 4. Configure Azure OpenAI Endpoint
Configure your Azure OpenAI endpoint for this project, using .NET User Secrets:
   1.  In the Azure Portal, navigate to your Azure OpenAI resource.
   2.  Copy the "Endpoint" URL from the "Keys and Endpoint" section.
   3.  In Visual Studio, right-click on your project in the Solution Explorer and select "Manage User Secrets".
   4.  This will open a `secrets.json` file where you can store your Azure OpenAI endpoint without it being tracked in source control. Add the following key and value to the file:

   ```json
   {
  "AzureOpenAI:Endpoint": "YOUR-AZURE-OPENAI-ENDPOINT"
   }
   ```

Make sure to replace `YOUR-AZURE-OPENAI-ENDPOINT` with your actual Azure OpenAI endpoint, formatted like https://YOUR-DEPLOYMENT-NAME.openai.azure.com/ (do not include any path after .openai.azure.com/).
