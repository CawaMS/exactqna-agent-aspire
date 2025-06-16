using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace ChatClient.Services;

public class Exact_QnA_Service
{
    HttpClient httpClient;
    private readonly ILogger<Exact_QnA_Service> _logger;

    public Exact_QnA_Service(HttpClient httpClient, ILogger<Exact_QnA_Service> logger)
    {
        this.httpClient = httpClient;
        _logger = logger;
    }

    
    public async Task<string> GetAnswerAsync(string question, CancellationToken cancellationToken)
    {

        var response = await httpClient.GetAsync($"/agent/answer?question={Uri.EscapeDataString(question)}", cancellationToken);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
        var answer = JsonSerializer.Deserialize<string>(jsonResponse);
        
        return answer ?? "System down. Cannot answer this question at this moment";
    }
}
