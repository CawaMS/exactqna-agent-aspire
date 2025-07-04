using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using ChatClient.Components;
using ChatClient.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddHttpClient<Exact_QnA_Service>(client =>
{
    client.BaseAddress = new("https+http://AgentAPI");
});

// You will need to set the endpoint and key to your own values
// You can do this using Visual Studio's "Manage User Secrets" UI, or on the command line:
//   cd this-project-directory
//   dotnet user-secrets set AzureOpenAI:Endpoint https://YOUR-DEPLOYMENT-NAME.openai.azure.com
var azureOpenAi = new AzureOpenAIClient(
    new Uri(builder.Configuration["ConnectionStrings:aoai"] ?? throw new InvalidOperationException("Missing configuration: AzureOpenAi:Endpoint. See the README for details.")),
    new DefaultAzureCredential());
var chatClient = azureOpenAi.AsChatClient(builder.Configuration["ConnectionStrings:chat-model-id"] ?? "gpt-4"); // "gpt-4"
var embeddingGenerator = azureOpenAi.AsEmbeddingGenerator(builder.Configuration["ConnectionStrings:embedding-model-id"] ?? "text-embedding-ada-002"); // "text-embedding-ada-002"

builder.Services.AddChatClient(chatClient).UseFunctionInvocation().UseLogging();
builder.Services.AddEmbeddingGenerator(embeddingGenerator);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// By default, we ingest PDF files from the /wwwroot/Data directory. You can ingest from
// other sources by implementing IIngestionSource.
// Important: ensure that any content you ingest is trusted, as it may be reflected back
// to users or could be a source of prompt injection risk.

app.Run();
