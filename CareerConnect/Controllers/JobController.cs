using CareerConnect.Models;
using CareerConnect.Services;
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

    [HttpPost("post")]
    public async Task<IActionResult> PostJob([FromBody] Job job)
    {
        var result = await _jobService.PostJobAsync(job);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListJobs()
    {
        var jobs = await _jobService.ListJobsAsync();
        return Ok(jobs);
    }
}