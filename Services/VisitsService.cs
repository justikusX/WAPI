using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WAPI.Dtos;
using WPFPoliclinic.Models;

namespace WAPI.Services;

public class VisitsService : IVisitsService
{
    private readonly PoliclinicContext _db;
    private readonly IMemoryCache _cache;
    private readonly ILogger<VisitsService> _logger;

    public VisitsService(
        PoliclinicContext db,
        IMemoryCache cache,
        ILogger<VisitsService> logger)
    {
        _db = db;
        _cache = cache;
        _logger = logger;
    }

    public async Task<PagedResult<VisitDto>> GetAllAsync(
        PaginationParams pagination,
        CancellationToken ct)
    {
        var query = _db.Visits
            .AsNoTracking()
            .OrderByDescending(v => v.VisitDate);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(v => new VisitDto(
                v.Id,
                v.DoctorId,
                v.PatientId,
                v.DiagnosisId,
                v.VisitDate))
            .ToListAsync(ct);

        return new PagedResult<VisitDto>
        {
            Items = items,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<VisitDetailsDto>> GetDetailsAsync(
        VisitFilterDto filter,
        PaginationParams pagination,
        CancellationToken ct)
    {
        var cacheKey = $"visit_details:{filter.From}:{filter.To}:{filter.DoctorId}:{filter.PatientId}:{pagination.Page}:{pagination.PageSize}";

        if (_cache.TryGetValue(cacheKey, out PagedResult<VisitDetailsDto>? cached) && cached is not null)
        {
            _logger.LogInformation("Visits details returned from memory cache");
            return cached;
        }

        var query = _db.Visits
            .AsNoTracking()
            .Include(v => v.Doctor)
            .Include(v => v.Patient)
            .Include(v => v.Diagnosis)
            .AsQueryable();

        if (filter.From is not null)
            query = query.Where(v => v.VisitDate >= filter.From);

        if (filter.To is not null)
            query = query.Where(v => v.VisitDate <= filter.To);

        if (filter.DoctorId is not null)
            query = query.Where(v => v.DoctorId == filter.DoctorId);

        if (filter.PatientId is not null)
            query = query.Where(v => v.PatientId == filter.PatientId);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(v => v.VisitDate)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(v => new VisitDetailsDto(
                v.Id,
                v.VisitDate,
                v.DoctorId,
                v.Doctor.FullName,
                v.Doctor.Specialty,
                v.PatientId,
                v.Patient.FullName,
                v.DiagnosisId,
                v.Diagnosis.Code,
                v.Diagnosis.Name))
            .ToListAsync(ct);

        var result = new PagedResult<VisitDetailsDto>
        {
            Items = items,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };

        _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(30),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            Size = 1
        });

        return result;
    }

    public async Task<VisitDto> CreateAsync(CreateVisitDto dto, CancellationToken ct)
    {
        var ok = await _db.Doctors.AnyAsync(x => x.Id == dto.DoctorId, ct)
            && await _db.Patients.AnyAsync(x => x.Id == dto.PatientId, ct)
            && await _db.Diagnoses.AnyAsync(x => x.Id == dto.DiagnosisId, ct);

        if (!ok)
            throw new InvalidOperationException("Invalid DoctorId/PatientId/DiagnosisId");

        var entity = new Visit
        {
            DoctorId = dto.DoctorId,
            PatientId = dto.PatientId,
            DiagnosisId = dto.DiagnosisId,
            VisitDate = dto.VisitDate
        };

        _db.Visits.Add(entity);
        await _db.SaveChangesAsync(ct);

        return new VisitDto(
            entity.Id,
            entity.DoctorId,
            entity.PatientId,
            entity.DiagnosisId,
            entity.VisitDate);
    }

    public async Task<bool> UpdateAsync(int id, UpdateVisitDto dto, CancellationToken ct)
    {
        var entity = await _db.Visits.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null)
            return false;

        var ok = await _db.Doctors.AnyAsync(x => x.Id == dto.DoctorId, ct)
            && await _db.Patients.AnyAsync(x => x.Id == dto.PatientId, ct)
            && await _db.Diagnoses.AnyAsync(x => x.Id == dto.DiagnosisId, ct);

        if (!ok)
            throw new InvalidOperationException("Invalid DoctorId/PatientId/DiagnosisId");

        entity.DoctorId = dto.DoctorId;
        entity.PatientId = dto.PatientId;
        entity.DiagnosisId = dto.DiagnosisId;
        entity.VisitDate = dto.VisitDate;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await _db.Visits.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null)
            return false;

        _db.Visits.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}