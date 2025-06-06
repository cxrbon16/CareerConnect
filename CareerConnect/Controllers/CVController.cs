﻿using CareerConnect.Models;
using CareerConnect.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "JobSeekersOnly")] //Sadece Job Seeker'lar ile sınırlandırıldı
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

    [HttpGet("{cvId}")]
    public IActionResult GetCV(int cvId)
    {
        var cv = _cvService.GetCVById(cvId);
        if (cv == null)
            return NotFound("CV bulunamadı.");

        return Ok(cv);
    }

    [HttpPut("update/{cvId}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateCV([FromRoute] int cvId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Dosya yüklenmedi.");

        var existingCv = _cvService.GetCVById(cvId);
        if (existingCv == null)
            return NotFound("Güncellenecek CV bulunamadı.");

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var fileBytes = ms.ToArray();

        var updatedCv = await _cvService.UpdateCVAsync(cvId, fileBytes);
        return Ok(updatedCv);
    }

    [HttpDelete("{cvId}")]
    public IActionResult DeleteCV(int cvId)
    {
        var result = _cvService.DeleteCV(cvId);
        if (!result)
            return NotFound("Silinecek CV bulunamadı.");

        return NoContent();
    }

    [HttpGet("user/{userId}")]
    public IActionResult GetUserCVs(int userId)
    {
        var cvs = _cvService.GetCVsByUserId(userId);
        if (cvs == null || !cvs.Any())
            return NotFound("Kullanıcıya ait CV bulunamadı.");

        return Ok(cvs);
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

    [HttpGet("score/{cvId}/{jobId}")]
    public IActionResult GetCVJobScore(int cvId, int jobId)
    {
        var cv = _cvService.GetCVById(cvId);
        if (cv == null)
            return NotFound("CV bulunamadı.");

        var score = _cvService.CalculateJobMatchScore(cvId, jobId);
        return Ok(new { cvId, jobId, score });
    }
}