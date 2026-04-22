using WAPI.Dtos;

namespace WAPI.Services;

public interface IPatientsService
{
    Task<PagedResult<PatientDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct);
    Task<PatientDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<PatientDto> CreateAsync(CreatePatientDto dto, CancellationToken ct);
    Task<bool> UpdateAsync(int id, UpdatePatientDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}