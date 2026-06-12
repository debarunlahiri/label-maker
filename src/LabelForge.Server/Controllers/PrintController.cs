using LabelForge.Database;
using LabelForge.Core.Models.Printing;
using LabelForge.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabelForge.Server.Controllers;

[ApiController]
[Route("api/print")]
[Authorize]
public class PrintController : ControllerBase
{
    private readonly LabelForgeDbContext _context;

    public PrintController(LabelForgeDbContext context)
    {
        _context = context;
    }

    [HttpPost("jobs")]
    public async Task<ActionResult<CreatePrintJobResponse>> CreateJob([FromBody] CreatePrintJobRequest request)
    {
        var template = await _context.Templates.FindAsync(request.TemplateId);
        if (template == null) return NotFound(new { Error = "Template not found" });

        if (template.Status != TemplateStatus.Approved)
            return BadRequest(new { Error = "Template is not approved for printing" });

        var printer = await _context.Printers.FindAsync(request.PrinterId);
        if (printer == null) return NotFound(new { Error = "Printer not found" });

        var job = new PrintJob
        {
            Id = Guid.NewGuid(),
            TemplateId = request.TemplateId,
            TemplateVersionId = template.CurrentVersionId ?? Guid.Empty,
            PrinterId = request.PrinterId,
            Copies = request.Copies,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(request.Data),
            Status = PrintJobStatus.Queued,
            RequestedSource = RequestedSource.RestApi,
            CreatedAt = DateTime.UtcNow
        };

        _context.PrintJobs.Add(job);
        await _context.SaveChangesAsync();

        return Ok(new CreatePrintJobResponse
        {
            JobId = job.Id,
            Status = job.Status.ToString(),
            Message = "Print job created successfully"
        });
    }

    [HttpGet("jobs/{jobId}")]
    public async Task<ActionResult<PrintJob>> GetJob(Guid jobId)
    {
        var job = await _context.PrintJobs
            .Include(j => j.Logs)
            .FirstOrDefaultAsync(j => j.Id == jobId);
        if (job == null) return NotFound();
        return Ok(job);
    }

    [HttpPost("jobs/{jobId}/cancel")]
    public async Task<ActionResult> CancelJob(Guid jobId)
    {
        var job = await _context.PrintJobs.FindAsync(jobId);
        if (job == null) return NotFound();
        if (job.Status == PrintJobStatus.Completed || job.Status == PrintJobStatus.Cancelled)
            return BadRequest(new { Error = "Job cannot be cancelled" });
        job.Status = PrintJobStatus.Cancelled;
        job.CompletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Job cancelled" });
    }

    [HttpPost("jobs/{jobId}/retry")]
    public async Task<ActionResult> RetryJob(Guid jobId)
    {
        var job = await _context.PrintJobs.FindAsync(jobId);
        if (job == null) return NotFound();
        if (job.Status != PrintJobStatus.Failed)
            return BadRequest(new { Error = "Only failed jobs can be retried" });
        job.Status = PrintJobStatus.Queued;
        job.ErrorMessage = null;
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Job queued for retry" });
    }

    [HttpGet("jobs")]
    public async Task<ActionResult<IEnumerable<PrintJob>>> GetJobs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var jobs = await _context.PrintJobs
            .OrderByDescending(j => j.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Ok(jobs);
    }
}

public class CreatePrintJobRequest
{
    public Guid TemplateId { get; set; }
    public Guid PrinterId { get; set; }
    public int Copies { get; set; } = 1;
    public Dictionary<string, object> Data { get; set; } = new();
}

public class CreatePrintJobResponse
{
    public Guid JobId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}