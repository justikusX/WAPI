using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WAPI.Dtos;
using WAPI.Services;

namespace WAPI.Controllers;

[ApiController]
[Route("api/doctors")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorsService _service;

    public DoctorsController(IDoctorsService service)
    {
        _service = service;
    }

    [HttpGet]
    [ResponseCache(Duration = 20, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "page", "pageSize" })]
    public async Task<ActionResult<PagedResult<DoctorDto>>> GetAll(
        [FromQuery] PaginationParams pagination,
        CancellationToken ct)
    {
        return Ok(await _service.GetAllAsync(pagination, ct));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DoctorDto>> GetById(int id, CancellationToken ct)
    {
        var doctor = await _service.GetByIdAsync(id, ct);
        return doctor is null ? NotFound() : Ok(doctor);
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpPost]
    public async Task<ActionResult<DoctorDto>> Create(CreateDoctorDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateDoctorDto dto, CancellationToken ct)
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