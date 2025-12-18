using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WAPI.Dtos;
using WPFPoliclinic.Models;

namespace WAPI.Controllers;

[ApiController]
[Route("api/visits")]
public class VisitsController : ControllerBase
{
    private readonly PoliclinicContext _db;
    public VisitsController(PoliclinicContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<VisitDto>>> GetAll(CancellationToken ct)
        => await _db.Visits.AsNoTracking()
            .OrderByDescending(v => v.VisitDate)
            .Select(v => new VisitDto(v.Id, v.DoctorId, v.PatientId, v.DiagnosisId, v.VisitDate))
            .ToListAsync(ct);

    [HttpGet("details")]
    public async Task<ActionResult<List<VisitDetailsDto>>> GetDetails(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] int? doctorId,
        [FromQuery] int? patientId,
        CancellationToken ct)
    {
        var q = _db.Visits.AsNoTracking()
            .Include(v => v.Doctor)
            .Include(v => v.Patient)
            .Include(v => v.Diagnosis)
            .AsQueryable();

        if (from is not null) q = q.Where(v => v.VisitDate >= from);
        if (to is not null) q = q.Where(v => v.VisitDate <= to);
        if (doctorId is not null) q = q.Where(v => v.DoctorId == doctorId);
        if (patientId is not null) q = q.Where(v => v.PatientId == patientId);

        return await q.OrderByDescending(v => v.VisitDate)
            .Select(v => new VisitDetailsDto(
                v.Id,
                v.VisitDate,
                v.DoctorId, v.Doctor.FullName, v.Doctor.Specialty,
                v.PatientId, v.Patient.FullName,
                v.DiagnosisId, v.Diagnosis.Code, v.Diagnosis.Name
            ))
            .ToListAsync(ct);
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpPost]
    public async Task<ActionResult<VisitDto>> Create(CreateVisitDto dto, CancellationToken ct)
    {
        var ok =
            await _db.Doctors.AnyAsync(x => x.Id == dto.DoctorId, ct) &&
            await _db.Patients.AnyAsync(x => x.Id == dto.PatientId, ct) &&
            await _db.Diagnoses.AnyAsync(x => x.Id == dto.DiagnosisId, ct);

        if (!ok) return BadRequest("Invalid DoctorId/PatientId/DiagnosisId");

        var entity = new Visit
        {
            DoctorId = dto.DoctorId,
            PatientId = dto.PatientId,
            DiagnosisId = dto.DiagnosisId,
            VisitDate = dto.VisitDate
        };

        _db.Visits.Add(entity);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetAll), new { id = entity.Id },
            new VisitDto(entity.Id, entity.DoctorId, entity.PatientId, entity.DiagnosisId, entity.VisitDate));
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateVisitDto dto, CancellationToken ct)
    {
        var entity = await _db.Visits.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();

        var ok =
            await _db.Doctors.AnyAsync(x => x.Id == dto.DoctorId, ct) &&
            await _db.Patients.AnyAsync(x => x.Id == dto.PatientId, ct) &&
            await _db.Diagnoses.AnyAsync(x => x.Id == dto.DiagnosisId, ct);

        if (!ok) return BadRequest("Invalid DoctorId/PatientId/DiagnosisId");

        entity.DoctorId = dto.DoctorId;
        entity.PatientId = dto.PatientId;
        entity.DiagnosisId = dto.DiagnosisId;
        entity.VisitDate = dto.VisitDate;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var entity = await _db.Visits.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();

        _db.Visits.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}