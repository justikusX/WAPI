using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WAPI.Dtos;
using WPFPoliclinic.Models;


namespace WAPI.Controllers;

[ApiController]
[Route("api/doctors")]
public class DoctorsController : ControllerBase
{
    private readonly PoliclinicContext _db;
    public DoctorsController(PoliclinicContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<DoctorDto>>> GetAll(CancellationToken ct)
        => await _db.Doctors.AsNoTracking()
            .OrderBy(d => d.LastName).ThenBy(d => d.FirstName)
            .Select(d => new DoctorDto(d.Id, d.LastName, d.FirstName, d.Patronymic, d.Specialty, d.Experience))
            .ToListAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DoctorDto>> GetById(int id, CancellationToken ct)
    {
        var d = await _db.Doctors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return d is null ? NotFound() : new DoctorDto(d.Id, d.LastName, d.FirstName, d.Patronymic, d.Specialty, d.Experience);
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpPost]
    public async Task<ActionResult<DoctorDto>> Create(CreateDoctorDto dto, CancellationToken ct)
    {
        var entity = new Doctor
        {
            LastName = dto.LastName,
            FirstName = dto.FirstName,
            Patronymic = dto.Patronymic,
            Specialty = dto.Specialty,
            Experience = dto.Experience
        };

        _db.Doctors.Add(entity);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id },
            new DoctorDto(entity.Id, entity.LastName, entity.FirstName, entity.Patronymic, entity.Specialty, entity.Experience));
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateDoctorDto dto, CancellationToken ct)
    {
        var entity = await _db.Doctors.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();

        entity.LastName = dto.LastName;
        entity.FirstName = dto.FirstName;
        entity.Patronymic = dto.Patronymic;
        entity.Specialty = dto.Specialty;
        entity.Experience = dto.Experience;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var entity = await _db.Doctors.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();

        _db.Doctors.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}