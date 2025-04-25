using CareerConnect.Data;
using CareerConnect.Models;
using CareerConnect.Models.DTOs;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace CareerConnect.Services;

public class CVService
{
    private readonly ApplicationDbContext _context;
    private readonly PdfExtractor _pdfExtractor;

    public CVService(ApplicationDbContext context)
    {
        _context = context;
        _pdfExtractor = new PdfExtractor();
    }
    private string ExtractJsonFromText(string text)
    {
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');

        if (start == -1 || end == -1 || end <= start)
        {
            throw new Exception("No valid JSON object found in model output.");
        }

        return text.Substring(start, end - start + 1);
    }
    public async Task<ParsedCV> ParseCvWithApiAsync(string text)
    {
        using var client = new HttpClient();

        /*
        var requestBody = new { text = text };
        var response = await client.PostAsJsonAsync("http://localhost:8000/parse-cv", requestBody);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Parsing API error: {error}");
        }
        
        var parsedCv = await response.Content.ReadFromJsonAsync<ParsedCV>();
        return parsedCv!;
        */


        var payload = new
        {
            model = "phi",
            prompt = $@"You are an AI assistant. Extract the Skills, Education, and Experience from the following CV text.
                Return the output strictly as a JSON object with ONLY one field: ""skills"". And the content of this field have to be a list of strings. Each string should 1-3 words.
                Can contain programming languages, softwares, tools, etc. Do not use single quotes, use double quotes.

                The JSON object should look like this:
                {{
                    ""skills"": [""skill1"", ""skill2"", ...]
                }}

                CV Text:
                {text}",
            stream = false,
        };

        HttpResponseMessage response;

        try
        {
            response = await client.PostAsJsonAsync("http://localhost:11434/api/generate", payload);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to connect to Ollama server.", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new Exception($"Ollama API Error ({response.StatusCode}): {errorBody}");
        }

        JsonObject? root;
        var x = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"[DEBUG] Raw Ollama Response: {x}");

        try
        {
            root = await response.Content.ReadFromJsonAsync<JsonObject>();
        }
        catch (Exception ex)
        {
            var rawContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to parse JSON from Ollama response. Raw Content: {rawContent}", ex);
        }

        if (root == null || !root.ContainsKey("response"))
        {
            throw new Exception("Ollama response does not contain a 'response' field.");
        }

        var modelResponseText = root["response"]?.ToString();

        if (string.IsNullOrWhiteSpace(modelResponseText))
        {
            throw new Exception("Ollama returned an empty 'response'.");
        }

        string cleanJson;

        try
        {
            cleanJson = ExtractJsonFromText(modelResponseText!);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to extract JSON from model response text. Text: {modelResponseText}", ex);
        }

        try
        {
            var parsed = JsonSerializer.Deserialize<ParsedCV>(cleanJson);
            return parsed!;
        }
        catch (JsonException ex)
        {
            throw new Exception($"Failed to deserialize extracted JSON: {cleanJson}", ex);
        }
    }

        

    public async Task<CV> UploadAndParseAsync(int userId, byte[] fileBytes)
    {
        // Dummy parse
        // Gerçek parsing fonksiyonu burada olacak. Burası değişecek.

        var cvText = await _pdfExtractor.PdfToTextAsync(fileBytes);
        var parseCv = await ParseCvWithApiAsync(cvText);

        var cv = new CV
        {
            UserId = userId,
            Skills = parseCv.Skills,
            Education = [],
            Experience = [],
        };

        _context.CVs.Add(cv);
        await _context.SaveChangesAsync();

        return cv;
    }

    public List<JobMatch> MatchJobs(CV cv)
    {
        return cv.MatchJobs();
    }

    public CV? GetCVById(int id) => _context.CVs.FirstOrDefault(c => c.Id == id);

    // Get all CVs for a specific user
    public List<CV> GetCVsByUserId(int userId)
    {
        return _context.CVs
                       .Where(c => c.UserId == userId)
                       .ToList();
    }

    // Update CV
    public async Task<CV?> UpdateCVAsync(int cvId, byte[] fileBytes)
    {
        var existingCv = await _context.CVs.FindAsync(cvId);
        if (existingCv == null)
            return null;

        // Parse the new CV data
        existingCv.Skills = new List<string> { "UpdatedSkill1", "UpdatedSkill2" };
        existingCv.Education = new List<string> { "Updated Education" };
        existingCv.Experience = new List<string> { "Updated Experience" };
        //existingCv.FileBytes = fileBytes; existingCv.UpdatedDate = DateTime.UtcNow;

        _context.CVs.Update(existingCv);
        await _context.SaveChangesAsync();

        return existingCv;
    }

    // Delete CV
    public bool DeleteCV(int cvId)
    {
        var cv = _context.CVs.FirstOrDefault(c => c.Id == cvId);
        if (cv == null)
            return false;

        _context.CVs.Remove(cv);
        _context.SaveChanges();
        return true;
    }

    //Calculate job match score
    public int CalculateJobMatchScore(int cvId, int jobId)
    {
        var cv = GetCVById(cvId);
        if (cv == null)
            return 0;

        var matches = cv.MatchJobs();
        var match = matches.FirstOrDefault(m => m.JobId == jobId);
        return match?.Score ?? 0;
    }
}