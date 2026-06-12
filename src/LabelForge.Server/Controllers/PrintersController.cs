using LabelForge.Database;
using LabelForge.Core.Models.Printing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabelForge.Server.Controllers;

[ApiController]
[Route("api/printers")]
[Authorize]
public class PrintersController : ControllerBase
{
    private readonly LabelForgeDbContext _context;

    public PrintersController(LabelForgeDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PrinterInfo>>> GetAll()
    {
        var printers = await _context.Printers.Where(p => p.IsActive).ToListAsync();
        return Ok(printers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PrinterInfo>> GetById(Guid id)
    {
        var printer = await _context.Printers.FindAsync(id);
        if (printer == null) return NotFound();
        return Ok(printer);
    }

    [HttpGet("{id}/status")]
    public async Task<ActionResult> GetStatus(Guid id)
    {
        var printer = await _context.Printers.FindAsync(id);
        if (printer == null) return NotFound();
        return Ok(new { PrinterId = id, printer.Status, printer.LastSeen });
    }

    [HttpPost]
    public async Task<ActionResult<PrinterInfo>> Register([FromBody] PrinterInfo printer)
    {
        printer.LastSeen = DateTime.UtcNow;
        _context.Printers.Add(printer);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = printer.Id }, printer);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PrinterInfo>> Update(Guid id, [FromBody] PrinterInfo printer)
    {
        var existing = await _context.Printers.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Name = printer.Name;
        existing.PrinterType = printer.PrinterType;
        existing.ConnectionType = printer.ConnectionType;
        existing.IpAddress = printer.IpAddress;
        existing.Port = printer.Port;
        existing.DriverName = printer.DriverName;
        existing.Dpi = printer.Dpi;
        existing.Location = printer.Location;
        existing.Department = printer.Department;
        existing.LastSeen = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var printer = await _context.Printers.FindAsync(id);
        if (printer == null) return NotFound();
        printer.IsActive = false;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}