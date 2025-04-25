namespace CareerConnect.Models;

public class CV
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<string>? Education { get; set; } = new();
    public List<string>? Experience { get; set; } = new();

    public List<JobMatch> MatchJobs()
    {
        // Dummy eşleştirme
        return new List<JobMatch>
        {
            new JobMatch { JobId = 1, Score = 95 },
            new JobMatch { JobId = 2, Score = 87 }
        };
    }
}

public class JobMatch
{
    public int JobId { get; set; }
    public int Score { get; set; }
}