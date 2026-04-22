using Microsoft.EntityFrameworkCore;
using WAPI.Dtos;
using WPFPoliclinic.Models;

namespace WAPI.Services;

public class PatientsService : IPatientsService
{
    private readonly PoliclinicContext _db;

    public PatientsService(PoliclinicContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<PatientDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct)
    {
        var query = _db.Patients
            .AsNoTracking()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(p => new PatientDto(
                p.Id,
                p.LastName,
                p.FirstName,
                p.Patronymic,
                p.BirthDate,
                p.Address))
            .ToListAsync(ct);

        return new PagedResult<PatientDto>
        {
            Items = items,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PatientDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Patients
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(p => new PatientDto(
                p.Id,
                p.LastName,
                p.FirstName,
                p.Patronymic,
                p.BirthDate,
                p.Address))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<PatientDto> CreateAsync(CreatePatientDto dto, CancellationToken ct)
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

        return new PatientDto(
            entity.Id,
            entity.LastName,
            entity.FirstName,
            entity.Patronymic,
            entity.BirthDate,
            entity.Address);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePatientDto dto, CancellationToken ct)
    {
        var entity = await _db.Patients.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null)
            return false;

        entity.LastName = dto.LastName;
        entity.FirstName = dto.FirstName;
        entity.Patronymic = dto.Patronymic;
        entity.BirthDate = dto.BirthDate;
        entity.Address = dto.Address;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await _db.Patients.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null)
            return false;

        _db.Patients.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}