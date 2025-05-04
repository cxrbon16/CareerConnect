using CareerConnect.Models;
using CareerConnect.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly JobService _jobService;

        public JobController(JobService jobService)
        {
            _jobService = jobService;
        }

        // İşverenler yeni ilan ekler
        [Authorize(Policy = "EmployersOnly")]
        [HttpPost("post")]
        public async Task<IActionResult> PostJob([FromBody] Job job)
        {
            var result = await _jobService.PostJobAsync(job);
            return Ok(result);
        }

        // Tüm ilanları listele (herkes görebilir)
        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<IActionResult> ListJobs()
        {
            var jobs = await _jobService.ListJobsAsync();
            return Ok(jobs);
        }

        // Tek bir ilanı getir
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
                return NotFound();
            return Ok(job);
        }

        [Authorize(Policy = "JobSeekersOnly")]
        [HttpPost("{id}/apply")]
        public async Task<IActionResult> ApplyToJob(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var seekerId))
                return Unauthorized();

            var success = await _jobService.ApplyAsync(id, seekerId);
            if (!success)
                return BadRequest("Zaten bu ilana başvurdunuz veya ilan bulunamadı.");

            return Ok(new { message = "Başvuru başarılı" });
        }

        // İşverenler başvuran adayları görüntüler
        [Authorize(Policy = "EmployersOnly")]
        [HttpGet("{id}/applicants")]
        public async Task<IActionResult> GetApplicantsForJob(int id)
        {
            var applicants = await _jobService.GetApplicantsAsync(id);
            return Ok(applicants);
        }

        // İşveren ilanı siler
        [Authorize(Policy = "EmployersOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var success = await _jobService.DeleteJobAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }

        [Authorize(Policy = "JobSeekersOnly")]
        [HttpGet("relevant")]
        public async Task<IActionResult> GetRelevantJobs()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var seekerId))
                return Unauthorized();

            var relevantJobs = await _jobService.ListJobsAsync(); // geçici bir şekilde tüm ilanlar listelensin
            return Ok(relevantJobs);
        }

        [Authorize(Policy = "EmployersOnly")]
        [HttpGet("employer/{employerId}")]
        public async Task<IActionResult> GetJobsByEmployerId(int employerId)
        {
            var jobs = await _jobService.GetJobsByEmployerIdAsync(employerId);
            return Ok(jobs);
        }


    }
}
