using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using AgentAPI.Agents;
using AgentAPI.Agents.Plugins;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable SKEXP0010

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddTransient<KBPlugin>();

using var tempProvider = builder.Services.BuildServiceProvider();
// Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddKernel()
                .AddAzureOpenAIChatCompletion(builder.Configuration.GetConnectionString("aoai-chat-deployment") ?? string.Empty,
                                              builder.Configuration.GetConnectionString("aoai-endpoint") ?? string.Empty,
                                              builder.Configuration.GetConnectionString("aoai-key") ?? string.Empty)
                .Plugins.AddFromObject(tempProvider.GetRequiredService<KBPlugin>())
                ;

builder.Services.AddTransient<ExactQnA_Agent>();



// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/agent/answer", async ([FromQuery]string question, ExactQnA_Agent agent) =>
{
    if (string.IsNullOrWhiteSpace(question))
    {
        return Results.BadRequest("Question cannot be empty.");
    }
    var answer = await agent.GetAnswerAsync(question);
    return Results.Ok(answer);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
