using WAPI.Dtos;

namespace WAPI.Services;

public interface IDoctorsService
{
    Task<PagedResult<DoctorDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct);
    Task<DoctorDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<DoctorDto> CreateAsync(CreateDoctorDto dto, CancellationToken ct);
    Task<bool> UpdateAsync(int id, UpdateDoctorDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}