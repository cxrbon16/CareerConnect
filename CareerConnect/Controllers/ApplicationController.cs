using CareerConnect.Models;
using CareerConnect.Services;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly ApplicationService _applicationService;

    public ApplicationController(ApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpPost("apply")]
    public async Task<IActionResult> Apply([FromQuery] int jobId, [FromQuery] int seekerId)
    {
        try
        {
            var application = await _applicationService.ApplyAsync(jobId, seekerId);
            return Ok(application);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("candidates/{jobId}")]
    public async Task<IActionResult> GetCandidates(int jobId)
    {
        var candidates = await _applicationService.GetCandidatesForJobAsync(jobId);
        return Ok(candidates);
    }

    [HttpGet("user/{seekerId}")]
    public async Task<IActionResult> GetApplicationsByUser(int seekerId)
    {
        var applications = await _applicationService.GetUserApplicationsWithJobsAsync(seekerId);
        return Ok(applications);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllApplications() {
        var applications = await _applicationService.GetAllApplicationsAsync();
        return Ok(applications);
    }
}
