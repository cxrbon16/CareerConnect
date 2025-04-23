using CareerConnect.Data;
using CareerConnect.Models;
using CareerConnect.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.Services;

public class ApplicationService
{
    private readonly ApplicationDbContext _context;

    public ApplicationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Application> ApplyAsync(int jobId, int seekerId)
    {
        bool alreadyExists = await _context.Applications
            .AnyAsync(a => a.Id == jobId && a.SeekerId == seekerId);

        if (alreadyExists)
            throw new InvalidOperationException("Zaten başvuru yapılmış.");

        bool jobDoesntExist = !(await _context.Jobs
            .AnyAsync(a => a.Id == jobId));

        if (jobDoesntExist)
            throw new InvalidOperationException($"{jobId} id'sine sahip iş ilanı kayıtlı değil.");

        bool userDoesntExist = !(await _context.Users
            .AnyAsync(a => a.Id == seekerId));

        if (userDoesntExist)
            throw new InvalidOperationException($"{seekerId} id'sine sahip kullanıcı kayıtlı değil.");

        var application = new Application
        {
            JobId = jobId,
            SeekerId = seekerId
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        return application;
    }

    public async Task<List<Application>> GetCandidatesForJobAsync(int jobId)
    {
        return await _context.Applications
            .Where(a => a.JobId == jobId)
            .ToListAsync();
    }

    public async Task<List<UserApplication>> GetUserApplicationsWithJobsAsync(int seekerId)
    {
        var applications = await _context.Applications
            .Where(a => a.SeekerId == seekerId)
            .ToListAsync();

        var jobIds = applications.Select(a => a.JobId).Distinct().ToList();
        var jobs = await _context.Jobs
            .Where(j => jobIds.Contains(j.Id))
            .ToListAsync();

        var result = applications.Select(a => new UserApplication
        {
            ApplicationId = a.Id,
            AppliedAt = a.AppliedAt,
            Job = jobs.First(j => j.Id == a.JobId)
        }).ToList();

        return result;
    }
}