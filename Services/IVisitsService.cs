using WAPI.Dtos;

namespace WAPI.Services;

public interface IVisitsService
{
    Task<PagedResult<VisitDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct);
    Task<PagedResult<VisitDetailsDto>> GetDetailsAsync(VisitFilterDto filter, PaginationParams pagination, CancellationToken ct);
    Task<VisitDto> CreateAsync(CreateVisitDto dto, CancellationToken ct);
    Task<bool> UpdateAsync(int id, UpdateVisitDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}