using CareerConnect.Data;
using CareerConnect.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.Services;

public class JobService
{
    private readonly ApplicationDbContext _context;

    public JobService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Job> PostJobAsync(Job job)
    {
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        return job;
    }

    public async Task<List<Job>> ListJobsAsync()
    {
        return await _context.Jobs.ToListAsync();
    }
}