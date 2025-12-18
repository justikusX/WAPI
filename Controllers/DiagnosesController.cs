using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WAPI.Dtos;
using WPFPoliclinic.Models;

namespace WAPI.Controllers;

[ApiController]
[Route("api/diagnoses")]
public class DiagnosesController : ControllerBase
{
    private readonly PoliclinicContext _db;
    public DiagnosesController(PoliclinicContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<DiagnosisDto>>> GetAll(CancellationToken ct)
        => await _db.Diagnoses.AsNoTracking()
            .OrderBy(d => d.Code)
            .Select(d => new DiagnosisDto(d.Id, d.Code, d.Name))
            .ToListAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DiagnosisDto>> GetById(int id, CancellationToken ct)
    {
        var d = await _db.Diagnoses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return d is null ? NotFound() : new DiagnosisDto(d.Id, d.Code, d.Name);
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpPost]
    public async Task<ActionResult<DiagnosisDto>> Create(CreateDiagnosisDto dto, CancellationToken ct)
    {
        
        if (await _db.Diagnoses.AnyAsync(x => x.Code == dto.Code, ct))
            return Conflict("Diagnosis code already exists");

        var entity = new Diagnosis { Code = dto.Code, Name = dto.Name };
        _db.Diagnoses.Add(entity);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new DiagnosisDto(entity.Id, entity.Code, entity.Name));
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateDiagnosisDto dto, CancellationToken ct)
    {
        var entity = await _db.Diagnoses.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();

        if (entity.Code != dto.Code && await _db.Diagnoses.AnyAsync(x => x.Code == dto.Code, ct))
            return Conflict("Diagnosis code already exists");

        entity.Code = dto.Code;
        entity.Name = dto.Name;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var entity = await _db.Diagnoses.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();

        _db.Diagnoses.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}