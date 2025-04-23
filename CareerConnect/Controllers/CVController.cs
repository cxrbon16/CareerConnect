using CareerConnect.Models;
using CareerConnect.Services;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CVController : ControllerBase
{
    private readonly CVService _cvService;

    public CVController(CVService cvService)
    {
        _cvService = cvService;
    }

    [HttpPost("upload/{userId}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCV([FromRoute] int userId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Dosya yüklenmedi.");

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var fileBytes = ms.ToArray();

        // CV'yi işleyecek cvService.UploadAndParseAsync fonksiyonu
        var cv = await _cvService.UploadAndParseAsync(userId, fileBytes);
        return Ok(cv);
    }

    [HttpGet("match-jobs/{cvId}")]
    public IActionResult MatchJobs(int cvId)
    {
        var cv = _cvService.GetCVById(cvId); // Bunu servise ekleyeceğiz
        if (cv == null)
            return NotFound();

        var matches = _cvService.MatchJobs(cv);
        return Ok(matches);
    }
}