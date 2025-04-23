using CareerConnect.Data;
using CareerConnect.Models;

namespace CareerConnect.Services;

public class CVService
{
    private readonly ApplicationDbContext _context;

    public CVService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CV> UploadAndParseAsync(int userId, byte[] fileBytes)
    {
        // Dummy parse
        // Gerçek parsing fonksiyonu burada olacak. Burası değişecek.
        var cv = new CV
        {
            UserId = userId,
            Skills = new List<string> { "C#", "ASP.NET", "SQL" },
            Education = new List<string> { "BS Computer Engineering" },
            Experience = new List<string> { "3 years at ABC Software" }
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
}