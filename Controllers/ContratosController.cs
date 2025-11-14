using Inmobiliaria.Api.Data;
using Inmobiliaria.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inmobiliaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContratosController : ControllerBase {
    private readonly AppDbContext _db;
    public ContratosController(AppDbContext db) { _db = db; }

    // GET /api/contratos/inmueble/{id}
    [HttpGet("inmueble/{id:int}")]
    public async Task<ActionResult<ContratoDto>> GetByInmueble(int id) {
        var now = DateOnly.FromDateTime(DateTime.Today);
        var c = await _db.Contratos
            .Include(x => x.Inquilino)
            .Include(x => x.Inmueble)!.ThenInclude(i => i!.Duenio)
            .FirstOrDefaultAsync(x => x.IdInmueble == id);

        if (c is null) return NotFound();

        var vigente = c.Estado && c.FechaInicio <= now && c.FechaFinalizacion >= now;
        return Ok(ContratoDto.FromEntity(c, vigente));
    }
}