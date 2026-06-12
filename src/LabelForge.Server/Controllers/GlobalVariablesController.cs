using LabelForge.Database;
using LabelForge.Core.Models.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabelForge.Server.Controllers;

[ApiController]
[Route("api/global-variables")]
[Authorize]
public class GlobalVariablesController : ControllerBase
{
    private readonly LabelForgeDbContext _context;

    public GlobalVariablesController(LabelForgeDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GlobalVariable>>> GetAll()
    {
        var variables = await _context.GlobalVariables.Where(v => v.IsActive).ToListAsync();
        return Ok(variables);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GlobalVariable>> GetById(Guid id)
    {
        var variable = await _context.GlobalVariables.FindAsync(id);
        if (variable == null) return NotFound();
        return Ok(variable);
    }

    [HttpPost]
    public async Task<ActionResult<GlobalVariable>> Create([FromBody] GlobalVariable variable)
    {
        variable.CreatedAt = DateTime.UtcNow;
        _context.GlobalVariables.Add(variable);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = variable.Id }, variable);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GlobalVariable>> Update(Guid id, [FromBody] GlobalVariable variable)
    {
        var existing = await _context.GlobalVariables.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Name = variable.Name;
        existing.Value = variable.Value;
        existing.DataType = variable.DataType;
        existing.Description = variable.Description;
        existing.ModifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var variable = await _context.GlobalVariables.FindAsync(id);
        if (variable == null) return NotFound();
        variable.IsActive = false;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}