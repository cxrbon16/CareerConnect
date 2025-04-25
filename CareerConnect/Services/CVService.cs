using CareerConnect.Data;
using CareerConnect.Models;
using CareerConnect.Models.DTOs;

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

    public async Task<ParsedCV> ParseCvWithApiAsync(string text)
    {
        using var client = new HttpClient();

        var requestBody = new { text = text };

        var response = await client.PostAsJsonAsync("http://localhost:8000/parse-cv", requestBody);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Parsing API error: {error}");
        }

        var parsedCv = await response.Content.ReadFromJsonAsync<ParsedCV>();
        return parsedCv!;
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
            Education = parseCv.Education,
            Experience = parseCv.Experience,
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