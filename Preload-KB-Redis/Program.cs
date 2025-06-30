using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Redis.OM;
using Redis.OM.Vectorizers;

// Get configuration object
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// Replace with your values.
string embeddingDeploymentName = config["AOAI:embeddingDeploymentName"] ?? "";
string endpoint = config["AOAI:endpoint"] ?? "";
string apiKey = config["AOAI:apiKey"] ?? "";
string redisConnectionString = config["Redis:connectionString"] ?? "";
string semanticCacheRedisProvider = config["Redis:SemanticCacheAzureProvider"] ?? "";
string cognitiveAccountName = config["AOAIResourceName"] ?? "";


var _provider = new RedisConnectionProvider(semanticCacheRedisProvider);
var cache = _provider.AzureOpenAISemanticCache(apiKey, cognitiveAccountName, embeddingDeploymentName, 1536, threshold: 0.30);

await cache.StoreAsync("What is your refund policy for returning a product and get my money back from Contoso Outdoors?", "Contoso Outdoors is proud to offer a 30 day refund policy. Return unopened, unsused products within 30 days of purchase to any Contoso Outdoors store for a full refund.");
await cache.StoreAsync("Refund policy", "Contoso Outdoors is proud to offer a 30 day refund policy. Return unopened, unsused products within 30 days of purchase to any Contoso Outdoors store for a full refund.");
await cache.StoreAsync("Contoso Outdoors refund policy", "Contoso Outdoors is proud to offer a 30 day refund policy. Return unopened, unsused products within 30 days of purchase to any Contoso Outdoors store for a full refund.");

await cache.StoreAsync("Rental Policy", "Contoso Outdoors offers rentals for a variety of outdoor products. Stop by your local Contoso Outdoors store to consult with an employee about their regional rental offerings.");
await cache.StoreAsync("What is your rental policy?", "Contoso Outdoors offers rentals for a variety of outdoor products. Stop by your local Contoso Outdoors store to consult with an employee about their regional rental offerings.");
await cache.StoreAsync("Contoso Outdoors rental policy", "Contoso Outdoors offers rentals for a variety of outdoor products. Stop by your local Contoso Outdoors store to consult with an employee about their regional rental offerings.");

await cache.StoreAsync("Annual Sales", "Each year, Contoso Outdoors hosts its annual summer sale. Expect big discounts on your favorite products so that you can get outside with comfort and style!");
await cache.StoreAsync("Does Contoso Outdoors offer any sales?", "Each year, Contoso Outdoors hosts its annual summer sale. Expect big discounts on your favorite products so that you can get outside with comfort and style!");
await cache.StoreAsync("When are Contoso Outdoors sales?", "Each year, Contoso Outdoors hosts its annual summer sale. Expect big discounts on your favorite products so that you can get outside with comfort and style!");

await cache.StoreAsync("Summer Programs", "Contoso Outdoors offers five exciting summer program options for people of all ages. Stop by your local store to see regional offerings and dates!");
await cache.StoreAsync("Does Contoso Outdoors have any summer programs?", "Contoso Outdoors offers five exciting summer program options for people of all ages. Stop by your local store to see regional offerings and dates!");

await cache.StoreAsync("Rewards Program", "Join Contoso Outdoor's rewards program today. Enjoy benefits at every tier, including extra discounts during sales. Please consult with your local store to become a member.");
await cache.StoreAsync("Is there a rewards program at Contoso Outdoors?", "Join Contoso Outdoor's rewards program today. Enjoy benefits at every tier, including extra discounts during sales. Please consult with your local store to become a member.");
await cache.StoreAsync("Can I earn rewards?", "Join Contoso Outdoor's rewards program today. Enjoy benefits at every tier, including extra discounts during sales. Please consult with your local store to become a member.");


//var response = await cache.GetSimilarAsync("what is your refund policy?");
//Console.WriteLine("Question: " + "what is your refund policy?");
//Console.WriteLine("Response length: " + response.Length);
