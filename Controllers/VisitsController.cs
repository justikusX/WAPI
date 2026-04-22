using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WAPI.Dtos;
using WAPI.Services;

namespace WAPI.Controllers;

[ApiController]
[Route("api/visits")]
public class VisitsController : ControllerBase
{
    private readonly IVisitsService _service;

    public VisitsController(IVisitsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<VisitDto>>> GetAll(
        [FromQuery] PaginationParams pagination,
        CancellationToken ct)
    {
        var result = await _service.GetAllAsync(pagination, ct);
        return Ok(result);
    }

    [HttpGet("details")]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "from", "to", "doctorId", "patientId", "page", "pageSize" })]
    public async Task<ActionResult<PagedResult<VisitDetailsDto>>> GetDetails(
        [FromQuery] VisitFilterDto filter,
        [FromQuery] PaginationParams pagination,
        CancellationToken ct)
    {
        var result = await _service.GetDetailsAsync(filter, pagination, ct);
        return Ok(result);
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpPost]
    public async Task<ActionResult<VisitDto>> Create(CreateVisitDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateVisitDto dto, CancellationToken ct)
    {
        var ok = await _service.UpdateAsync(id, dto, ct);
        return ok ? NoContent() : NotFound();
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}