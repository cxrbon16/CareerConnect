using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

public class OpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAIService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["OpenAI:ApiKey"];
    }

    public async Task<string> ExtractCvFieldsAsync(string cvText)
    {
        var prompt = $@"
You are an AI assistant. Extract the Skills, Education and Experience from the following CV text. 
Return the output strictly as a JSON object with fields 'skills', 'education', and 'experience'.

Here is an example:

CV Text:
John Doe is a software engineer with 5 years of experience. He has worked at Google as a backend developer between 2018 and 2021.
He also worked at Meta from 2021 to 2023. He graduated from MIT with a BSc in Computer Science in 2016.
His skills include Python, Django, and PostgreSQL.

Output:
{{
  ""skills"": [""Python"", ""Django"", ""PostgreSQL""],
  ""education"": [""MIT"", ""BSc"", ""Computer Science""],
  ""experience"": [""Google"", ""Meta"", ""Backend Developer""]
}}

Now, extract the same fields from this CV:

CV Text:
{cvText}
";

        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = "You extract structured data from CVs." },
                new { role = "user", content = prompt }
            },
            temperature = 0.2
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"OpenAI API error: {response.StatusCode}\n{content}");

        using var doc = JsonDocument.Parse(content);
        var completion = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return completion!;
    }
}
