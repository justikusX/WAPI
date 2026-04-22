using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WAPI.Dtos;
using WPFPoliclinic.Models;

namespace WAPI.Services;

public class DoctorsService : IDoctorsService
{
    private readonly PoliclinicContext _db;
    private readonly IMemoryCache _cache;

    public DoctorsService(PoliclinicContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<PagedResult<DoctorDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct)
    {
        var cacheKey = $"doctors:{pagination.Page}:{pagination.PageSize}";

        if (_cache.TryGetValue(cacheKey, out PagedResult<DoctorDto>? cached) && cached is not null)
            return cached;

        var query = _db.Doctors
            .AsNoTracking()
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(d => new DoctorDto(
                d.Id,
                d.LastName,
                d.FirstName,
                d.Patronymic,
                d.Specialty,
                d.Experience))
            .ToListAsync(ct);

        var result = new PagedResult<DoctorDto>
        {
            Items = items,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };

        _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(20),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
            Size = 1
        });

        return result;
    }

    public async Task<DoctorDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Doctors
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(d => new DoctorDto(
                d.Id,
                d.LastName,
                d.FirstName,
                d.Patronymic,
                d.Specialty,
                d.Experience))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<DoctorDto> CreateAsync(CreateDoctorDto dto, CancellationToken ct)
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

        return new DoctorDto(
            entity.Id,
            entity.LastName,
            entity.FirstName,
            entity.Patronymic,
            entity.Specialty,
            entity.Experience);
    }

    public async Task<bool> UpdateAsync(int id, UpdateDoctorDto dto, CancellationToken ct)
    {
        var entity = await _db.Doctors.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null)
            return false;

        entity.LastName = dto.LastName;
        entity.FirstName = dto.FirstName;
        entity.Patronymic = dto.Patronymic;
        entity.Specialty = dto.Specialty;
        entity.Experience = dto.Experience;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await _db.Doctors.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null)
            return false;

        _db.Doctors.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}