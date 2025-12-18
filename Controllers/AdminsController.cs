using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WAPI.Dtos;
using WPFPoliclinic.Models;

namespace WAPI.Controllers;

[ApiController]
[Route("api/admins")]
[Authorize(Roles = "superadmin")]
public class AdminsController : ControllerBase
{
    private readonly PoliclinicContext _db;
    public AdminsController(PoliclinicContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<AdminDto>>> GetAll(CancellationToken ct)
        => await _db.Admins.AsNoTracking()
            .OrderBy(a => a.Login)
            .Select(a => new AdminDto(a.Id, a.Login, a.FirstName, a.LastName, a.CreatedAt, a.LastLogin, a.IsActive, a.Role))
            .ToListAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdminDto>> GetById(int id, CancellationToken ct)
    {
        var a = await _db.Admins.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return a is null ? NotFound() : new AdminDto(a.Id, a.Login, a.FirstName, a.LastName, a.CreatedAt, a.LastLogin, a.IsActive, a.Role);
    }

    [HttpPost]
    public async Task<ActionResult<AdminDto>> Create(CreateAdminDto dto, CancellationToken ct)
    {
        if (await _db.Admins.AnyAsync(x => x.Login == dto.Login, ct))
            return Conflict("Login already exists");

        var admin = new WPFPoliclinic.Models.Admin
        {
            Login = dto.Login,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IsActive = dto.IsActive,
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };
        admin.SetPassword(dto.Password);

        _db.Admins.Add(admin);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = admin.Id },
            new AdminDto(admin.Id, admin.Login, admin.FirstName, admin.LastName, admin.CreatedAt, admin.LastLogin, admin.IsActive, admin.Role));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateAdminDto dto, CancellationToken ct)
    {
        var admin = await _db.Admins.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (admin is null) return NotFound();

        admin.FirstName = dto.FirstName;
        admin.LastName = dto.LastName;
        admin.IsActive = dto.IsActive;
        admin.Role = dto.Role;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPut("{id:int}/password")]
    public async Task<IActionResult> ChangePassword(int id, ChangePasswordDto dto, CancellationToken ct)
    {
        var admin = await _db.Admins.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (admin is null) return NotFound();

        admin.SetPassword(dto.NewPassword);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var admin = await _db.Admins.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (admin is null) return NotFound();

        _db.Admins.Remove(admin);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}