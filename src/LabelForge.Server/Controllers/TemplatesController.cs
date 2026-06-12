using LabelForge.Database;
using LabelForge.Core.Models.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabelForge.Server.Controllers;

[ApiController]
[Route("api/templates")]
[Authorize]
public class TemplatesController : ControllerBase
{
    private readonly LabelForgeDbContext _context;

    public TemplatesController(LabelForgeDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LabelTemplate>>> GetAll()
    {
        var templates = await _context.Templates
            .OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt)
            .ToListAsync();
        return Ok(templates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LabelTemplate>> GetById(Guid id)
    {
        var template = await _context.Templates
            .Include(t => t.Elements)
            .Include(t => t.Versions)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (template == null) return NotFound();
        return Ok(template);
    }

    [HttpPost]
    public async Task<ActionResult<LabelTemplate>> Create([FromBody] LabelTemplate template)
    {
        template.CreatedAt = DateTime.UtcNow;
        _context.Templates.Add(template);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = template.Id }, template);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LabelTemplate>> Update(Guid id, [FromBody] LabelTemplate template)
    {
        var existing = await _context.Templates.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Name = template.Name;
        existing.Description = template.Description;
        existing.Width = template.Width;
        existing.Height = template.Height;
        existing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var template = await _context.Templates.FindAsync(id);
        if (template == null) return NotFound();
        _context.Templates.Remove(template);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/preview")]
    public async Task<ActionResult> Preview(Guid id, [FromBody] Dictionary<string, object> data)
    {
        var template = await _context.Templates.FindAsync(id);
        if (template == null) return NotFound();
        return Ok(new { TemplateId = id, Preview = "Preview generation not yet implemented" });
    }

    [HttpGet("{id}/versions")]
    public async Task<ActionResult<IEnumerable<TemplateVersion>>> GetVersions(Guid id)
    {
        var versions = await _context.TemplateVersions
            .Where(v => v.TemplateId == id)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();
        return Ok(versions);
    }

    [HttpPost("{id}/submit")]
    public async Task<ActionResult> SubmitForApproval(Guid id, [FromBody] SubmitVersionRequest request)
    {
        var template = await _context.Templates.FindAsync(id);
        if (template == null) return NotFound();
        template.Status = Core.Enums.TemplateStatus.PendingApproval;
        template.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Template submitted for approval" });
    }

    [HttpPost("{id}/approve")]
    public async Task<ActionResult> Approve(Guid id)
    {
        var template = await _context.Templates.FindAsync(id);
        if (template == null) return NotFound();
        template.Status = Core.Enums.TemplateStatus.Approved;
        template.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Template approved" });
    }

    [HttpPost("{id}/reject")]
    public async Task<ActionResult> Reject(Guid id, [FromBody] RejectRequest request)
    {
        var template = await _context.Templates.FindAsync(id);
        if (template == null) return NotFound();
        template.Status = Core.Enums.TemplateStatus.Rejected;
        template.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Template rejected" });
    }
}

public class SubmitVersionRequest
{
    public string? ChangeComment { get; set; }
}

public class RejectRequest
{
    public string? Reason { get; set; }
}