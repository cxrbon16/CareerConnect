using Microsoft.EntityFrameworkCore.Query;
using CareerConnect.Models;

public class JobMatcherService
{
    public List<JobMatch> Match(CV cv, List<Job> allJobs)
    {
        var matches = new List<JobMatch>();

        foreach (var job in allJobs)
        {
            int matchedSkillCount = job.Skills
                .Intersect(cv.Skills, StringComparer.OrdinalIgnoreCase)
                .Count();

            int score = job.Skills.Count > 0
                ? (int)((matchedSkillCount / (double)job.Skills.Count) * 100)
                : 0;

            matches.Add(new JobMatch
            {
                Job = job,
                Score = score
            });
        }

        return matches.OrderByDescending(m => m.Score).ToList();
    }
}