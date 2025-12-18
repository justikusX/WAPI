using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WAPI.Dtos;
using WPFPoliclinic.Models;

namespace WAPI.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly PoliclinicContext _db;
    public PatientsController(PoliclinicContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<PatientDto>>> GetAll(CancellationToken ct)
        => await _db.Patients.AsNoTracking()
            .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .Select(p => new PatientDto(p.Id, p.LastName, p.FirstName, p.Patronymic, p.BirthDate, p.Address))
            .ToListAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PatientDto>> GetById(int id, CancellationToken ct)
    {
        var p = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return p is null ? NotFound() : new PatientDto(p.Id, p.LastName, p.FirstName, p.Patronymic, p.BirthDate, p.Address);
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpPost]
    public async Task<ActionResult<PatientDto>> Create(CreatePatientDto dto, CancellationToken ct)
    {
        var entity = new Patient
        {
            LastName = dto.LastName,
            FirstName = dto.FirstName,
            Patronymic = dto.Patronymic,
            BirthDate = dto.BirthDate,
            Address = dto.Address
        };

        _db.Patients.Add(entity);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id },
            new PatientDto(entity.Id, entity.LastName, entity.FirstName, entity.Patronymic, entity.BirthDate, entity.Address));
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdatePatientDto dto, CancellationToken ct)
    {
        var entity = await _db.Patients.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();

        entity.LastName = dto.LastName;
        entity.FirstName = dto.FirstName;
        entity.Patronymic = dto.Patronymic;
        entity.BirthDate = dto.BirthDate;
        entity.Address = dto.Address;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var entity = await _db.Patients.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();

        _db.Patients.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}