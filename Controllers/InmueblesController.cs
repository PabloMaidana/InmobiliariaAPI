using Inmobiliaria.Api.Data;
using Inmobiliaria.Api.Dtos;
using Inmobiliaria.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Inmobiliaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InmueblesController : ControllerBase {
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _cfg;

    public InmueblesController(AppDbContext db, IWebHostEnvironment env, IConfiguration cfg) {
        _db = db; _env = env; _cfg = cfg;
    }

    private async Task<int> GetOwnerIdAsync() {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var p = await _db.Propietarios.FirstAsync(x => x.Email == email);
        return p.IdPropietario;
    }

    // GET /api/Inmuebles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InmuebleDto>>> GetAll() {
        var ownerId = await GetOwnerIdAsync();
        var inm = await _db.Inmuebles
            .Include(i => i.Duenio)
            .Include(i => i.Contratos)
            .Where(i => i.IdPropietario == ownerId)
            .ToListAsync();

        var now = DateOnly.FromDateTime(DateTime.Today);
        var list = inm.Select(i => {
            var vigente = i.Contratos.Any(c => c.Estado && c.FechaInicio <= now && c.FechaFinalizacion >= now);
            return InmuebleDto.FromEntity(i, vigente);
        });
        return Ok(list);
    }

    // GET /api/Inmuebles/GetContratoVigente
    [HttpGet("GetContratoVigente")]
    public async Task<ActionResult<IEnumerable<InmuebleDto>>> GetVigentes() {
        var ownerId = await GetOwnerIdAsync();
        var inm = await _db.Inmuebles
            .Include(i => i.Duenio)
            .Include(i => i.Contratos)
            .Where(i => i.IdPropietario == ownerId)
            .ToListAsync();

        var now = DateOnly.FromDateTime(DateTime.Today);
        var list = inm
            .Where(i => i.Contratos.Any(c => c.Estado && c.FechaInicio <= now && c.FechaFinalizacion >= now))
            .Select(i => InmuebleDto.FromEntity(i, true));
        return Ok(list);
    }

    // POST /api/Inmuebles/cargar  (multipart/form-data)
    [HttpPost("cargar")]
    [Authorize]
    [RequestSizeLimit(20_000_000)]
    public async Task<ActionResult<InmuebleDto>> Cargar(
        [FromForm] IFormFile? imagen,
        [FromForm] string inmueble)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var propietario = await _db.Propietarios.FirstOrDefaultAsync(p => p.Email == email);
        if (propietario is null) return Unauthorized();

        // parsear el JSON que viene del form
        var entity = JsonConvert.DeserializeObject<Inmueble>(inmueble);
        if (entity is null) return BadRequest("Formato de inmueble inválido.");
        entity.IdPropietario = propietario.IdPropietario;

        // guardar la imagen si se envio
        if (imagen is not null && imagen.Length > 0)
        {
            var folder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imagen.FileName)}";
            var path = Path.Combine(folder, fileName);

            using var fs = new FileStream(path, FileMode.Create);
            await imagen.CopyToAsync(fs);

            entity.Imagen = $"uploads/{fileName}";
        }

        _db.Inmuebles.Add(entity);
        await _db.SaveChangesAsync();

        // cargar relaciones
        entity = await _db.Inmuebles
            .Include(i => i.Duenio)
            .FirstAsync(i => i.IdInmueble == entity.IdInmueble);

        return Ok(InmuebleDto.FromEntity(entity, false));
    }

    // PUT /api/Inmuebles/actualizar (JSON)
    [HttpPut("actualizar")]
    public async Task<ActionResult<InmuebleDto>> Actualizar([FromBody] Inmueble model)
    {
        var ownerId = await GetOwnerIdAsync();
        var entity = await _db.Inmuebles.Include(i => i.Duenio).Include(i => i.Contratos)
            .FirstOrDefaultAsync(i => i.IdInmueble == model.IdInmueble && i.IdPropietario == ownerId);

        if (entity is null) return NotFound();

        entity.Direccion = model.Direccion;
        entity.Uso = model.Uso;
        entity.Tipo = model.Tipo;
        entity.Ambientes = model.Ambientes;
        entity.Superficie = model.Superficie;
        entity.Latitud = model.Latitud;
        entity.Longitud = model.Longitud;
        entity.Valor = model.Valor;
        entity.Disponible = model.Disponible;

        await _db.SaveChangesAsync();

        var now = DateOnly.FromDateTime(DateTime.Today);
        var vigente = entity.Contratos.Any(c => c.Estado && c.FechaInicio <= now && c.FechaFinalizacion >= now);
        return Ok(InmuebleDto.FromEntity(entity, vigente));
    }
    
    // GET /api/Inmuebles/{id}
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<InmuebleDto>> GetById(int id)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var propietario = await _db.Propietarios.FirstOrDefaultAsync(p => p.Email == email);
        if (propietario is null) return Unauthorized();

        var inmueble = await _db.Inmuebles
            .Include(i => i.Duenio)
            .Include(i => i.Contratos)
            .FirstOrDefaultAsync(i => i.IdInmueble == id && i.IdPropietario == propietario.IdPropietario);

        if (inmueble is null) return NotFound("Inmueble no encontrado.");

        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var tieneVigente = inmueble.Contratos.Any(c => c.Estado && c.FechaInicio <= hoy && c.FechaFinalizacion >= hoy);

        return Ok(InmuebleDto.FromEntity(inmueble, tieneVigente));
    }
}