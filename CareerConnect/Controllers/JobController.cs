using CareerConnect.Models;
using CareerConnect.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobController : ControllerBase
{
    private readonly JobService _jobService;

    public JobController(JobService jobService)
    {
        _jobService = jobService;
    }
    [Authorize(Policy = "EmployersOnly")] // İş ilanlarını sadece employer eklemeli 
    [HttpPost("post")]
    public async Task<IActionResult> PostJob([FromBody] Job job)
    {
        var result = await _jobService.PostJobAsync(job);
        return Ok(result);
    }

    [AllowAnonymous] // İş ilanları herkese açık ise (olmayadabilir)
    [HttpGet("all")]
    public async Task<IActionResult> ListJobs()
    {
        var jobs = await _jobService.ListJobsAsync();
        return Ok(jobs);
    }
}