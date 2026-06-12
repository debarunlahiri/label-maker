using LabelForge.Database;
using LabelForge.Core.Models.Automation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabelForge.Server.Controllers;

[ApiController]
[Route("api/integrations")]
[Authorize]
public class IntegrationsController : ControllerBase
{
    private readonly LabelForgeDbContext _context;

    public IntegrationsController(LabelForgeDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Integration>>> GetAll()
    {
        var integrations = await _context.Integrations
            .Include(i => i.Triggers)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
        return Ok(integrations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Integration>> GetById(Guid id)
    {
        var integration = await _context.Integrations
            .Include(i => i.Triggers)
            .Include(i => i.Logs)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (integration == null) return NotFound();
        return Ok(integration);
    }

    [HttpPost]
    public async Task<ActionResult<Integration>> Create([FromBody] Integration integration)
    {
        integration.CreatedAt = DateTime.UtcNow;
        _context.Integrations.Add(integration);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = integration.Id }, integration);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Integration>> Update(Guid id, [FromBody] Integration integration)
    {
        var existing = await _context.Integrations.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Name = integration.Name;
        existing.Description = integration.Description;
        existing.Configuration = integration.Configuration;
        existing.FieldMapping = integration.FieldMapping;
        existing.IsActive = integration.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var integration = await _context.Integrations.FindAsync(id);
        if (integration == null) return NotFound();
        _context.Integrations.Remove(integration);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}/logs")]
    public async Task<ActionResult<IEnumerable<IntegrationLog>>> GetLogs(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var logs = await _context.IntegrationLogs
            .Where(l => l.IntegrationId == id)
            .OrderByDescending(l => l.StartedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Ok(logs);
    }
}