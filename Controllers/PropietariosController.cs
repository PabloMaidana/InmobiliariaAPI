using Inmobiliaria.Api.Data;
using Inmobiliaria.Api.Dtos;
using Inmobiliaria.Api.Models;
using Inmobiliaria.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Inmobiliaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropietariosController : ControllerBase {
    private readonly AppDbContext _db;
    private readonly PasswordService _pwd;
    private readonly JwtService _jwt;

    public PropietariosController(AppDbContext db, PasswordService pwd, JwtService jwt) {
        _db = db; _pwd = pwd; _jwt = jwt;
    }

    // POST /api/Propietarios/login (x-www-form-urlencoded)
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromForm] string Usuario, [FromForm] string Clave) {
        var p = await _db.Propietarios.FirstOrDefaultAsync(x => x.Email == Usuario);
        if (p is null || p.ClaveSalt is null || !_pwd.Verify(Clave, p.ClaveHash, p.ClaveSalt))
            return Unauthorized("Credenciales inválidas");

        var token = _jwt.CreateToken(p.IdPropietario, p.Email);
        return Ok(token);
    }

    // GET /api/Propietarios (perfil del autenticado)
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PropietarioDto>> GetPerfil() {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var p = await _db.Propietarios.FirstOrDefaultAsync(x => x.Email == email);
        if (p is null) return NotFound();

        return Ok(new PropietarioDto(p.IdPropietario, p.Nombre, p.Apellido, p.Dni, p.Telefono, p.Email, p.ClaveHash));
    }

    // PUT /api/Propietarios/actualizar (JSON)
    [Authorize]
    [HttpPut("actualizar")]
    public async Task<ActionResult<PropietarioDto>> Actualizar([FromBody] PropietarioDto dto) {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var p = await _db.Propietarios.FirstOrDefaultAsync(x => x.Email == email);
        if (p is null) return NotFound();

        p.Nombre = dto.Nombre; p.Apellido = dto.Apellido; p.Dni = dto.Dni;
        p.Telefono = dto.Telefono;
        await _db.SaveChangesAsync();

        return Ok(new PropietarioDto(p.IdPropietario, p.Nombre, p.Apellido, p.Dni, p.Telefono, p.Email, p.ClaveHash));
    }

    // POST /api/Propietarios/email  (reset password - x-www-form-urlencoded)
    [HttpPost("email")]
    public async Task<ActionResult<string>> Resetear([FromForm] string email) {
        var p = await _db.Propietarios.FirstOrDefaultAsync(x => x.Email == email);
        if (p is null) return Ok("Si el email existe, se enviará un mensaje con instrucciones.");


        var temp = Guid.NewGuid().ToString("N")[..8];
        var (hash, salt) = _pwd.HashPassword(temp);
        p.ClaveHash = hash; p.ClaveSalt = salt;
        await _db.SaveChangesAsync();


        return Ok("Se envió un correo para reestablecer la contraseña.");
    }

    // PUT /api/Propietarios/changePassword  (x-www-form-urlencoded)
    [Authorize]
    [HttpPut("changePassword")]
    public async Task<IActionResult> ChangePassword([FromForm] string currentPassword, [FromForm] string newPassword) {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var p = await _db.Propietarios.FirstOrDefaultAsync(x => x.Email == email);
        if (p is null || p.ClaveSalt is null || !_pwd.Verify(currentPassword, p.ClaveHash, p.ClaveSalt))
            return BadRequest("Contraseña actual incorrecta");

        var (hash, salt) = _pwd.HashPassword(newPassword);
        p.ClaveHash = hash; p.ClaveSalt = salt;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}