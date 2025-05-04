using CareerConnect.Data;
using CareerConnect.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareerConnect.Services
{
    public class JobService
    {
        private readonly ApplicationDbContext _context;

        public JobService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1) Yeni iş ilanı ekle
        public async Task<Job> PostJobAsync(Job job)
        {
            
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }

        // 2) Tüm ilanları listele
        public async Task<IEnumerable<Job>> ListJobsAsync()
        {
            return await _context.Jobs
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        // 3) Tek bir ilan getir
        public async Task<Job?> GetJobByIdAsync(int jobId)
        {
            return await _context.Jobs
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(j => j.Id == jobId);
        }

        // 4) Başvuru yap
        public async Task<bool> ApplyAsync(int jobId, int seekerId)
        {
            // İlanın varlığı
            if (!await _context.Jobs.AnyAsync(j => j.Id == jobId))
                return false;

            // Aynı iş için tekrar başvuru engelle
            if (await _context.Applications
                              .AnyAsync(a => a.JobId == jobId && a.SeekerId == seekerId))
                return false;

            var application = new Application
            {
                JobId = jobId,
                SeekerId = seekerId,
                AppliedAt = DateTime.UtcNow
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return true;
        }

        // 5) Bir ilana yapılan başvuruları döndür
        public async Task<IEnumerable<Application>> GetApplicantsAsync(int jobId)
        {
            return await _context.Applications
                                 .Where(a => a.JobId == jobId)
                                 .ToListAsync();
        }

        // 6) İlan sil
        public async Task<bool> DeleteJobAsync(int jobId)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
                return false;

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Job>> GetJobsByEmployerIdAsync(int employerId)
        {
            return await _context.Jobs
                .Include(j => j.Applications)
                .Where(j => j.EmployerId == employerId)
                .ToListAsync();
        }

    }
}
