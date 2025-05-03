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
    private readonly OpenAIService _openAIService;

    public CVService(ApplicationDbContext context, OpenAIService oAIService)
    {
        _context = context;
        _pdfExtractor = new PdfExtractor();
        _openAIService = oAIService;
    }
    
    public async Task<CV> UploadAndParseAsync(int userId, byte[] fileBytes)
    {

        var cvText = await _pdfExtractor.PdfToTextAsync(fileBytes);
        var jsonString = await _openAIService.ExtractCvFieldsAsync(cvText);

        var parsed = JsonSerializer.Deserialize<ParsedCV>(jsonString);

        if (parsed == null)
            throw new Exception("Parsing failed.");

        var cv = new CV
        {
            UserId = userId,
            Skills = parsed.Skills,
            Education = parsed.Education,
            Experience = parsed.Experience
        };

        _context.CVs.Add(cv);
        await _context.SaveChangesAsync();

        return cv;
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

    
}