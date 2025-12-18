using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WAPI.Dtos;

using WPFPoliclinic.Models;

namespace WAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly PoliclinicContext _db;
    private readonly IConfiguration _cfg;

    public AuthController(PoliclinicContext db, IConfiguration cfg)
    {
        _db = db;
        _cfg = cfg;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest req, CancellationToken ct)
    {
        var admin = await _db.Admins.FirstOrDefaultAsync(a => a.Login == req.Login && a.IsActive, ct);
        if (admin is null) return Unauthorized();

        if (!admin.VerifyPassword(req.Password)) return Unauthorized();

        admin.LastLogin = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
            new(ClaimTypes.Name, admin.Login),
            new(ClaimTypes.Role, admin.Role ?? "admin")
        };

        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token));
    }
}